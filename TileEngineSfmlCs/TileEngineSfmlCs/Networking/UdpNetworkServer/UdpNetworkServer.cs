using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.Utils.RandomGenerators;
using UdpNetworkInterface;

namespace TileEngineSfmlCs.Networking.UdpNetworkServer
{
    public class UdpNetworkServer : INetworkServer
    {
        private UdpClient _udpClient;
        private List<IPEndPoint> _connections = new List<IPEndPoint>();
        private List<ulong> _connectionCodes = new List<ulong>();
        private Task _udpListenTask;
        private Task _retransmitTask;
        private int _port;

        private bool _listeningRunning;

        private Queue<int> _receiveIdQueue = new Queue<int>();
        private Queue<byte[]> _receiveDataQueue = new Queue<byte[]>();
        private Queue<int> _reconnectQueue = new Queue<int>();
        private Queue<int> _newConnectionQueue = new Queue<int>();
        private Queue<string> _usernamesQueue = new Queue<string>();
        private Queue<int> _disconnectQueue = new Queue<int>();

        private List<Retransmission> _retransmissionQueue = new List<Retransmission>();

        private RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

        public event Action<int, string> OnNewConnection;
        public event Action<int> OnDisconnect;
        public event Action<int> OnReconnect;
        public event Action<int, byte[]> OnDataReceived;

        public int RetransmissionCount { get; set; } = 10;
        public int RetransmissionPeriodMs { get; set; } = 2000;

        public int[] ConnectionIds => GetConnectionIds();

        public bool NewConnectionsEnabled { get; set; } = true;
        public bool IsListening => _udpListenTask != null && _udpListenTask.Status == TaskStatus.Running;

        private int[] GetConnectionIds()
        {
            List<int> result = new List<int>(_connections.Count);
            for(int i = 0; i < _connections.Count; i++)
            {
                if (_connections[i] != null)
                {
                    result.Add(i);
                }
            }

            return result.ToArray();
        }

        public void SendData(int connectionId, byte[] data, Reliability reliability = Reliability.Unreliable)
        {
            IPEndPoint target = _connections[connectionId];
            if (target == null)
            {
                // TODO Client not found
                return;
            }

            SendData(target, UdpCommand.Data, data, reliability);
        }

        private ulong GenerateConfirmationToken()
        {
            ulong token = RandomUtils.GetRandomUInt64();
            lock (_reconnectQueue)
            {
                while (_retransmissionQueue.Any(r => r.ConfirmationToken == token))
                {
                    token = RandomUtils.GetRandomUInt64();
                }
            }

            return token;
        }

        private void SendData(IPEndPoint endPoint, UdpCommand command, byte[] data, Reliability reliability)
        {
            int pos = 0;
            ulong token = reliability == Reliability.Reliable ? GenerateConfirmationToken() : 0;
            UdpPackage udpPackage = new UdpPackage(command, reliability, token, data);
            byte[] datagram = new byte[udpPackage.Length];

            udpPackage.ToByteArray(datagram, pos);
            pos += udpPackage.Length;

            if (reliability == Reliability.Reliable)
            {
                Retransmission retransmission = new Retransmission(endPoint, datagram, token, RetransmissionCount);
                lock (_retransmissionQueue)
                {
                    _retransmissionQueue.Add(retransmission);
                }
            }

            _udpClient.Send(datagram, datagram.Length, endPoint);
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
            _retransmitTask = new Task(RetransmissionLoop, TaskCreationOptions.LongRunning);
            _retransmitTask.Start();
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
                    string username = _usernamesQueue.Dequeue();
                    OnNewConnection?.Invoke(connectionId, username);
                }
            }

            lock (_reconnectQueue)
            {
                while (_reconnectQueue.Count > 0)
                {
                    int connectionId = _reconnectQueue.Dequeue();
                    OnReconnect?.Invoke(connectionId);
                }
            }

            lock (_disconnectQueue)
            {
                while (_disconnectQueue.Count > 0)
                {
                    int connectionId = _disconnectQueue.Dequeue();
                    OnDisconnect?.Invoke(connectionId);
                }
            }

            lock (_receiveIdQueue)
            {
                while (_receiveIdQueue.Count > 0)
                {
                    int connectionId = _receiveIdQueue.Dequeue();
                    byte[] data = _receiveDataQueue.Dequeue();
                    OnDataReceived?.Invoke(connectionId, data);
                }
            }
        }

      

        public void Dispose()
        {
            _udpClient?.Dispose();
            _retransmitTask?.Dispose();
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

            Debug.WriteLine("Generating code...");
            while (code == 0 || _connectionCodes.Contains(code))
            {
                byte[] randomBytes = new byte[sizeof(ulong)];
                _randomNumberGenerator.GetNonZeroBytes(randomBytes);
                code = BitConverter.ToUInt64(randomBytes, 0);
            }

            Debug.WriteLine("Code generated.");

            int index = 0;
            while (index < _connections.Count)
            {
                if (_connections[index] == null)
                {
                    break;
                }

                index++;
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
                try
                {
                    UdpListenIteration();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }
        private void UdpListenIteration()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);
            byte[] datagram = _udpClient.Receive(ref endPoint);

            int connectionId = GetConnectionId(endPoint);
            
            UdpPackage udpPackage = new UdpPackage();
            udpPackage.FromByteArray(datagram, 0);

            if (udpPackage.Reliability == Reliability.Reliable)
            {
                UdpPackage confirmationPackage = new UdpPackage(UdpCommand.Confirmation, Reliability.Unreliable, udpPackage.ConfirmationToken, null);
                byte[] confirmationPackageBytes = new byte[confirmationPackage.Length];
                confirmationPackage.ToByteArray(confirmationPackageBytes, 0);
                _udpClient.Send(confirmationPackageBytes, confirmationPackageBytes.Length, endPoint);
            }

            switch (udpPackage.Command)
            {
                case UdpCommand.Connect:
                    int pos = 0;
                    ulong connectionCode = BitConverter.ToUInt64(udpPackage.Payload, pos);
                    pos += sizeof(ulong);
                    int nameLength = BitConverter.ToInt32(udpPackage.Payload, pos);
                    pos += sizeof(int);
                    string username = Encoding.Unicode.GetString(udpPackage.Payload, pos, nameLength);
                    pos += nameLength;
                    if (connectionId != -1)
                    {
                        // Client is reconnecting
                        SendData(endPoint, UdpCommand.Connect, BitConverter.GetBytes(connectionCode), Reliability.Unreliable);
                        Debug.WriteLine($"Client {connectionId} (${endPoint}) reconnected");
                        lock (_reconnectQueue)
                        {
                            _reconnectQueue.Enqueue(connectionId);
                        }
                    }
                    else
                    {
                        // Check connection code. May be this is an old client trying to reconnect


                        int codeId = GetConnectionCodeId(connectionCode);
                        if (codeId != -1)
                        {
                            _connections[codeId] = endPoint;
                            SendData(endPoint, UdpCommand.Connect, BitConverter.GetBytes(connectionCode), Reliability.Unreliable);
                            // This client is reconnecting. We need to replace old EndPoint with new one
                            Debug.WriteLine(
                                $"Client {connectionId} (${endPoint}) reconnected with different EndPoint");
                            lock (_reconnectQueue)
                            {
                                _reconnectQueue.Enqueue(codeId);
                            }
                            break;
                        }
                        else if (NewConnectionsEnabled)
                        {
                            // New connection
                            int newId = InsertNewEndPoint(endPoint, out ulong code);
                            // Answering with private connection code and Connect command. This means that server accepted connection
                            SendData(endPoint, UdpCommand.Connect, BitConverter.GetBytes(code), Reliability.Unreliable);
                            Debug.WriteLine($"New client with id {newId} and name {username} ({endPoint}) connected");
                            lock (_newConnectionQueue)
                            {
                                _newConnectionQueue.Enqueue(newId);
                                _usernamesQueue.Enqueue(username);
                            }
                        }
                        else
                        {
                            // Client didn't provide connection code. Ignoring
                            Debug.WriteLine($"Client (${endPoint}) rejected. No connection code provided");
                        }

                    }

                    break;
                case UdpCommand.Disconnect:
                    Debug.WriteLine($"Client with id {connectionId} (${endPoint}) disconnected by client's will");
                    lock (_disconnectQueue)
                    {
                        _disconnectQueue.Enqueue(connectionId);
                    }

                    _connections[connectionId] = null;
                    break;
                case UdpCommand.Data:
                    Debug.WriteLine($"Client with id {connectionId} (${endPoint}) data received");
                    lock (_receiveDataQueue)
                    {
                        _receiveIdQueue.Enqueue(connectionId);
                        _receiveDataQueue.Enqueue(udpPackage.Payload);
                    }
                    break;
                case UdpCommand.Confirmation:
                    ulong token = udpPackage.ConfirmationToken;
                    lock (_retransmissionQueue)
                    {
                        int retransmissionIndex = _retransmissionQueue.FindIndex(r => r.ConfirmationToken == token && r.EndPoint.Equals(endPoint));
                        _retransmissionQueue.RemoveAt(retransmissionIndex);
                    }
                    LogManager.RuntimeLogger.Log($"Confirmation token {token} received!");
                    break;
                default:
                    Debug.WriteLine($"Unknown command {udpPackage.Command}");
                    break;
            }
        }

        private async void RetransmissionLoop()
        {
            while (_listeningRunning)
            {
                Retransmission retransmission = null;

                lock (_retransmissionQueue)
                {
                    if (_retransmissionQueue.Count > 0)
                    {
                        retransmission = _retransmissionQueue[0];
                    }
                }

                if (retransmission != null)
                {
                    LogManager.RuntimeLogger.Log($"[UdpNetworkServer] Retrying token {retransmission.ConfirmationToken}");
                    _udpClient.Send(retransmission.DataBuffer, retransmission.DataBuffer.Length,
                        retransmission.EndPoint);
                    retransmission.RetriesRemaining--;
                    if (retransmission.RetriesRemaining == 0)
                    {
                        LogManager.RuntimeLogger.Log($"[UdpNetworkServer] Retransmission failed. Too many retries");
                        // TODO Mark player as disconnected?
                        lock (_retransmissionQueue)
                        {
                            _retransmissionQueue.RemoveAt(0);
                        }
                    }
                }

                await Task.Delay(RetransmissionPeriodMs);
            }
        }
    }
}
