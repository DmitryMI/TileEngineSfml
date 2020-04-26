using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace UdpNetworkInterface.UdpNetworkServer
{
    public class UdpNetworkServer : INetworkServer
    {
        private UdpClient _udpClient;
        private List<IPEndPoint> _connections = new List<IPEndPoint>();
        private List<ulong> _connectionCodes = new List<ulong>();
        private Task _udpListenTask;
        private int _port;

        private bool _listeningRunning;

        private Queue<int> _receiveIdQueue = new Queue<int>();
        private Queue<byte[]> _receiveDataQueue = new Queue<byte[]>();
        private Queue<int> _reconnectQueue = new Queue<int>();
        private Queue<int> _newConnectionQueue = new Queue<int>();
        private Queue<int> _disconnectQueue = new Queue<int>();

        private RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

        public event Action<int> OnNewConnection;
        public event Action<int> OnDisconnect;
        public event Action<int> OnReconnect;
        public event Action<int, byte[]> OnDataReceived;
        public void SendData(int connectionId, byte[] data)
        {
            
        }

        private void SendData(IPEndPoint endPoint, UdpCommand command, byte[] data)
        {
            byte[] datagramm = new byte[data.Length + 1];
            datagramm[0] = (byte)command;
            Array.Copy(data, 0, datagramm, 1, data.Length);
            _udpClient.SendAsync(datagramm, datagramm.Length, endPoint);
        }

        public UdpNetworkServer(int port)
        {
            _port = port;
        }

        public void StartServer()
        {
            if (IsListening)
            {
                throw new InvalidOperationException("Server was already listening");
            }
            _udpClient = new UdpClient(_port, AddressFamily.InterNetwork);
            _listeningRunning = true;
            _udpListenTask = new Task(UpdListenLoop, TaskCreationOptions.LongRunning);
            _udpListenTask.Start();
        }

        public void StopServer()
        {
            _udpClient.Close();
            _listeningRunning = false;
            _udpListenTask.Wait();
            _udpListenTask = null;
        }

        public void Poll()
        {
            if (!IsListening)
            {
                _udpListenTask = null;
            }

            lock (_newConnectionQueue)
            {
                while (_newConnectionQueue.Count > 0)
                {
                    int connectionId = _newConnectionQueue.Dequeue();
                    OnNewConnection?.Invoke(connectionId);
                }
            }

            lock (_disconnectQueue)
            {
                while (_disconnectQueue.Count > 0)
                {
                    int connectionId = _disconnectQueue.Dequeue();
                    OnNewConnection?.Invoke(connectionId);
                }
            }

            lock (_receiveIdQueue)
            {
                while (_reconnectQueue.Count > 0)
                {
                    int connectionId = _reconnectQueue.Dequeue();
                    byte[] data = _receiveDataQueue.Dequeue();
                    OnDataReceived?.Invoke(connectionId, data);
                }
            }
        }

        public bool NewConnectionsEnabled { get; set; } = true;
        public bool IsListening => _udpListenTask != null && _udpListenTask.Status == TaskStatus.Running;

        public void Dispose()
        {
            _udpClient?.Dispose();
        }

        private int GetConnectionId(IPEndPoint endPoint)
        {
            for (var i = 0; i < _connections.Count; i++)
            {
                var connection = _connections[i];
                if(connection == null)
                    continue;
                
                if (connection.Address.Equals(endPoint.Address) && connection.Port == endPoint.Port)
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetConnectionCodeId(ulong code)
        {
            for (var i = 0; i < _connections.Count; i++)
            {
                var connectionCode = _connectionCodes[i];
                if (connectionCode == 0)
                    continue;

                if (connectionCode == code)
                {
                    return i;
                }
            }

            return -1;
        }

        private int InsertNewEndPoint(IPEndPoint endPoint, out ulong code)
        {
            code = 0;
            while (code == 0 || _connectionCodes.Contains(code))
            {
                byte[] randomBytes = new byte[sizeof(ulong)];
                _randomNumberGenerator.GetNonZeroBytes(randomBytes);
                code = BitConverter.ToUInt64(randomBytes, 0);
            }

            int index = 0;
            while (index < _connections.Count)
            {
                if (_connections[index] == null)
                {
                    break;
                }
            }

            if (index == _connections.Count)
            {
                _connections.Add(endPoint);
                _connectionCodes.Add(code);
            }
            else
            {
                _connections[index] = endPoint;
                _connectionCodes[index] = code;
            }

            return index;
        }

        private void UpdListenLoop()
        {
            while (_listeningRunning)
            {
                UdpListenIteration();
            }
        }
        private void UdpListenIteration()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);
            byte[] datagramm = _udpClient.Receive(ref endPoint);

            int connectionId = GetConnectionId(endPoint);
            byte[] data = new byte[datagramm.Length - 1];
            Array.Copy(datagramm, 1, data, 0, data.Length);

            UdpCommand command = (UdpCommand) data[0];
            switch (command)
            {
                case UdpCommand.Connect:
                    if (connectionId != -1)
                    {
                        // Client is reconnecting
                        Debug.WriteLine($"Client {connectionId} (${endPoint.ToString()}) reconnected");
                        lock (_reconnectQueue)
                        {
                            _reconnectQueue.Enqueue(connectionId);
                        }
                    }
                    else
                    {
                        // Check connection code. May be this is an old client trying to reconnect
                        if (data.Length >= sizeof(ulong))
                        {
                            ulong connectionCode = BitConverter.ToUInt64(data, 0);
                            int codeId = GetConnectionCodeId(connectionCode);
                            if (codeId != -1)
                            {
                                _connections[codeId] = endPoint;
                                // This client is reconnecting. We need to replace old EndPoint with new one
                                Debug.WriteLine($"Client {connectionId} (${endPoint.ToString()}) reconnected with different EndPoint");
                                lock (_reconnectQueue)
                                {
                                    _reconnectQueue.Enqueue(codeId);
                                }
                                break;
                            }
                            else if(NewConnectionsEnabled)
                            { 
                                // New connection
                                _connections.Add(endPoint);
                                int newId = InsertNewEndPoint(endPoint, out ulong code);
                                // Answering with private connection code and Connect command. This means that server accepted connection
                                SendData(endPoint, UdpCommand.Connect, BitConverter.GetBytes(code));
                                Debug.WriteLine($"New client with id {connectionId} (${endPoint.ToString()}) reconnected");
                                lock (_newConnectionQueue)
                                {
                                    _newConnectionQueue.Enqueue(newId);
                                }
                            }
                           
                        }
                        else
                        {
                            // Client didn't provide connection code. Ignoring
                            Debug.WriteLine($"Client (${endPoint.ToString()}) rejected. No connection code provided");
                        }
                        
                    }
                    break;
                case UdpCommand.Disconnect:
                    Debug.WriteLine($"Client with id {connectionId} (${endPoint.ToString()}) disconnected by client's will");
                    lock (_disconnectQueue)
                    {
                        _disconnectQueue.Enqueue(connectionId);
                    }

                    _connections[connectionId] = null;
                    break;
                case UdpCommand.Data:
                    Debug.WriteLine($"Client with id {connectionId} (${endPoint.ToString()}) data received");
                    lock (_receiveDataQueue)
                    {
                        _receiveIdQueue.Enqueue(connectionId);
                        _receiveDataQueue.Enqueue(data);
                    }
                    break;
            }
        }
    }
}
