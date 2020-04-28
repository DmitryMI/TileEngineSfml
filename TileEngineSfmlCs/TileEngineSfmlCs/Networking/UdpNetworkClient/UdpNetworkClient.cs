using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.Utils.RandomGenerators;
using UdpNetworkInterface.UdpNetworkClient;

namespace TileEngineSfmlCs.Networking.UdpNetworkClient
{
    public class UdpNetworkClient : INetworkClient, IDisposable
    {
        private UdpClient _udpClient;
        private Task _listeningTask;
        private Task _retransmissionTask;
        private bool _listeningRunning;
        private IPEndPoint _serverEndPoint;

        private bool _isConnected;
        private ulong _connectionCode;
        private Queue<UdpCommand> _commandQueue = new Queue<UdpCommand>();
        private Queue<byte[]> _dataQueue = new Queue<byte[]>();
        private List<Retransmission> _retransmissions = new List<Retransmission>();

        public bool IsConnected => _isConnected;

        public ulong ConnectionCode => _connectionCode;

        public int RetriesCount { get; set; } = 10;
        public int RetransmissionPeriodMs { get; set; } = 2000;


        public UdpNetworkClient(IPEndPoint serverEndPoint)
        {
            _udpClient = new UdpClient();
            _serverEndPoint = serverEndPoint;

            _listeningRunning = true;
            _listeningTask = new Task(ListeningLoop, TaskCreationOptions.LongRunning);
            _listeningTask.Start();
            _retransmissionTask = new Task(RetransmissionLoop);
            _retransmissionTask.Start();
        }

        public void Connect(string username, ulong connectionCode = 0)
        {
            byte[] nameBytes = Encoding.Unicode.GetBytes(username);
            int nameLength = nameBytes.Length;
            byte[] nameLengthBytes = BitConverter.GetBytes(nameLength);
            byte[] payload = new byte[sizeof(ulong) + nameLengthBytes.Length + nameBytes.Length];
            int pos = 0;
            byte[] codeBytes = BitConverter.GetBytes(connectionCode);
            Array.Copy(codeBytes, 0, payload, pos, codeBytes.Length);
            pos += codeBytes.Length;
            Array.Copy(nameLengthBytes, 0, payload, pos, nameLengthBytes.Length);
            pos += nameLengthBytes.Length;
            Array.Copy(nameBytes, 0, payload, pos, nameLength);
          
            UdpPackage package = new UdpPackage(UdpCommand.Connect, Reliability.Unreliable, 0, payload);
            byte[] datagram = new byte[package.Length];
            package.ToByteArray(datagram, 0);

            _udpClient.Send(datagram, datagram.Length, _serverEndPoint);
        }

        public void Disconnect()
        {
            _udpClient.Close();
            _listeningRunning = false;
        }

        private ulong GenerateConfirmationToken()
        {
            ulong token = RandomUtils.GetRandomUInt64();
            lock (_retransmissions)
            {
                while (_retransmissions.Any(r => r.ConfirmationToken == token))
                {
                    token = RandomUtils.GetRandomUInt64();
                }
            }

            return token;
        }

        public void Send(byte[] data, Reliability reliability = Reliability.Unreliable)
        {
            ulong token = reliability == Reliability.Reliable ? GenerateConfirmationToken() : 0;
            Debug.WriteLine($"Confirmation token generated: {token}. Reliability: {reliability}");

            UdpPackage package = new UdpPackage(UdpCommand.Data, reliability, token, data);
            byte[] datagram = new byte[package.Length];
            package.ToByteArray(datagram, 0);

            Retransmission retransmission = new Retransmission(_serverEndPoint, datagram, token, RetriesCount);
            if (reliability == Reliability.Reliable)
            {
                lock (_retransmissions)
                {
                    _retransmissions.Add(retransmission);
                }
            }

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

            UdpPackage udpPackage = new UdpPackage();
            udpPackage.FromByteArray(datagram, 0);

            Debug.WriteLine($"[UdpNetworkClient] Command {udpPackage.Command}");

            if (udpPackage.Reliability == Reliability.Reliable)
            {
                UdpPackage confirmationPackage = new UdpPackage(UdpCommand.Confirmation, Reliability.Unreliable, udpPackage.ConfirmationToken, null);
                byte[] confirmationPackageBytes = new byte[confirmationPackage.Length];
                confirmationPackage.ToByteArray(confirmationPackageBytes, 0);
                _udpClient.Send(confirmationPackageBytes, confirmationPackageBytes.Length, _serverEndPoint);
            }


            switch (udpPackage.Command)
            {
                case UdpCommand.Connect:
                    _isConnected = true;
                    _connectionCode = BitConverter.ToUInt64(udpPackage.Payload, 0);
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
                        _dataQueue.Enqueue(udpPackage.Payload);
                        Debug.WriteLine($"[UdpNetworkClient] Leaving LOCK");
                    }
                    break;
                case UdpCommand.Confirmation:
                    ulong token = udpPackage.ConfirmationToken;
                    Debug.WriteLine($"[UdpNetworkClient] Confirmation for token {token} received");
                    lock (_retransmissions)
                    {
                        int retransmissionIndex = _retransmissions.FindIndex(r => r.ConfirmationToken == token);
                        _retransmissions.RemoveAt(retransmissionIndex);
                    }
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

                lock (_retransmissions)
                {
                    if (_retransmissions.Count > 0)
                    {
                        retransmission = _retransmissions[0];
                    }
                }

                if (retransmission != null)
                {
                    LogManager.RuntimeLogger.Log($"[UdpNetworkClient] Retrying token {retransmission.ConfirmationToken}. {retransmission.RetriesRemaining} reties remaining");
                    _udpClient.Send(retransmission.DataBuffer, retransmission.DataBuffer.Length,
                        retransmission.EndPoint);
                    retransmission.RetriesRemaining--;
                    if (retransmission.RetriesRemaining == 0)
                    {
                        LogManager.RuntimeLogger.Log($"[UdpNetworkClient] Retransmission failed. Too many retries");
                        // TODO Report connection lost?
                        lock (_retransmissions)
                        {
                            _retransmissions.RemoveAt(0);
                        }
                    }
                }

                await Task.Delay(RetransmissionPeriodMs);
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
