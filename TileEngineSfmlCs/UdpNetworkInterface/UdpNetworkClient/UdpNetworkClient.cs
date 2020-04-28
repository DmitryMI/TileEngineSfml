using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            _udpClient = new UdpClient();
            _serverEndPoint = serverEndPoint;

            _listeningRunning = true;
            _listeningTask = new Task(ListeningLoop, TaskCreationOptions.LongRunning);
            _listeningTask.Start();
           
        }

        public void Connect(string username, ulong connectionCode = 0)
        {
            byte[] nameBytes = Encoding.Unicode.GetBytes(username);
            int nameLength = nameBytes.Length;
            byte[] nameLengthBytes = BitConverter.GetBytes(nameLength);
            byte[] datagram = new byte[1 + sizeof(ulong) + nameLengthBytes.Length + nameBytes.Length];
            int pos = 0;
            datagram[pos] = (byte) UdpCommand.Connect;
            pos += 1;
            byte[] codeBytes = BitConverter.GetBytes(connectionCode);
            Array.Copy(codeBytes, 0, datagram, pos, codeBytes.Length);
            pos += codeBytes.Length;
            Array.Copy(nameLengthBytes, 0, datagram, pos, nameLengthBytes.Length);
            pos += nameLengthBytes.Length;
            Array.Copy(nameBytes, 0, datagram, pos, nameLength);

            _udpClient.Send(datagram, datagram.Length, _serverEndPoint);
        }

        public void Disconnect()
        {
            _udpClient.Close();
            _listeningRunning = false;
        }

        public void Send(byte[] data)
        {
            byte[] datagram = new byte[1 + data.Length];
            datagram[0] = (byte)UdpCommand.Data;
            Array.Copy(data, 0, datagram, 1, data.Length);
            _udpClient.Send(datagram, datagram.Length, _serverEndPoint);
        }

        public void Poll()
        {
            //Debug.WriteLine($"Polling in thread {Thread.CurrentThread.ManagedThreadId} started");
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
                            //Debug.WriteLine($"[UdpNetworkClient] Invoking data handler. It is null: {OnDataReceived == null}");
                            OnDataReceived?.Invoke(data);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            //Debug.WriteLine($"Polling in thread {Thread.CurrentThread.ManagedThreadId} finished");
        }

        private void ListeningLoop()
        {
            while (_listeningRunning)
            {
                try
                {
                    ListeningIteration();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
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

            Debug.WriteLine($"[UdpNetworkClient] Command {command}");

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
                        Debug.WriteLine($"Enqueueing connection command");
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
                    Debug.WriteLine($"[UdpNetworkClient] Data from server received. Entering LOCK");
                    lock (_commandQueue)
                    {
                        Debug.WriteLine($"[UdpNetworkClient] LOCK occupied");
                        _commandQueue.Enqueue(UdpCommand.Data);
                        _dataQueue.Enqueue(data);
                        Debug.WriteLine($"[UdpNetworkClient] Leaving LOCK");
                    }
                    break;
                default:
                    Debug.WriteLine($"Unknown command {command}");
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
