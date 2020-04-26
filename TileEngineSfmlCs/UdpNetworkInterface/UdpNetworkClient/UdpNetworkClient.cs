using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpNetworkInterface.UdpNetworkClient
{
    public class UdpNetworkClient : INetworkClient, IDisposable
    {
        private UdpClient _udpClient;
        private Task _listeningTask;
        private bool _listeningRunning;
        private IPEndPoint _serverEndPoint;

        private bool _isConnected;
        private ulong _connectionCode;
        private Queue<UdpCommand> _commandQueue = new Queue<UdpCommand>();
        private Queue<byte[]> _dataQueue = new Queue<byte[]>();

        public bool IsConnected => _isConnected;

        public ulong ConnectionCode => _connectionCode;

        public UdpNetworkClient(IPEndPoint serverEndPoint)
        {
            _udpClient = new UdpClient(AddressFamily.InterNetwork);

            _listeningTask = new Task(ListeningLoop, TaskCreationOptions.LongRunning);
            _serverEndPoint = serverEndPoint;
        }

        public void Connect(ulong connectionCode = 0)
        {
            byte[] datagram = new byte[sizeof(ulong) + 1];
            datagram[0] = (byte)UdpCommand.Connect;
            _udpClient.SendAsync(datagram, datagram.Length, _serverEndPoint);
        }

        public void Disconnect()
        {
            _udpClient.Close();
            _listeningRunning = false;
        }

        public void Poll()
        {
            lock (_commandQueue)
            {
                while (_commandQueue.Count > 0)
                {
                    UdpCommand command = _commandQueue.Dequeue();
                    switch (command)
                    {
                        case UdpCommand.Connect:
                            OnConnectionAccepted?.Invoke();
                            break;
                        case UdpCommand.Disconnect:
                            OnDisconnect?.Invoke();
                            break;
                        case UdpCommand.Data:
                            byte[] data = _dataQueue.Dequeue();
                            OnDataReceived?.Invoke(data);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void ListeningLoop()
        {
            while (_listeningRunning)
            {
                ListeningIteration();
            }
        }

        private void ListeningIteration()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, _serverEndPoint.Port);
            byte[] datagram = _udpClient.Receive(ref sender);

            if (!Equals(sender.Address, _serverEndPoint.Address) || sender.Port != _serverEndPoint.Port)
            {
                // Ignore any data that was sent by anybody except server
                Debug.WriteLine("Datagram ignored");
                return;
            }

            UdpCommand command = (UdpCommand)datagram[0];

            byte[] data = new byte[datagram.Length - 1];
            Array.Copy(datagram, 1, data, 0, data.Length);

            switch (command)
            {
                case UdpCommand.Connect:
                    _isConnected = true;
                    _connectionCode = BitConverter.ToUInt64(data, 0);
                    Debug.WriteLine($"Connected to server with code {_connectionCode}");
                    lock (_commandQueue)
                    {
                        _commandQueue.Enqueue(UdpCommand.Connect);
                    }
                    break;
                case UdpCommand.Disconnect:
                    _isConnected = false;
                    Debug.WriteLine($"Server sent disconnect command");
                    lock (_commandQueue)
                    {
                        _commandQueue.Enqueue(UdpCommand.Disconnect);
                    }
                    break;
                case UdpCommand.Data:
                    Debug.WriteLine($"Data from server received");
                    lock (_commandQueue)
                    {
                        _commandQueue.Enqueue(UdpCommand.Data);
                        _dataQueue.Enqueue(data);
                    }
                    break;
            }
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
            _listeningTask?.Dispose();
        }

        public event Action OnConnectionAccepted;
        public event Action OnDisconnect;
        public event Action<byte[]> OnDataReceived;
    }
}
