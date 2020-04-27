using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Types;
using UdpNetworkInterface.UdpNetworkClient;

namespace TileEngineSfmlCs.GameManagement.ClientSide
{
    public class TileEngineClient
    {
        private INetworkClient _networkClient;
        private string _username;
        private bool _isConnected;
        private bool _connectionPending;
        private float _connectionTimeout;

        public bool IsConnected => _isConnected;

        public event Action OnConnectionAcceptedEvent;
        public event Action OnConnectionTimeoutEvent;
        public event Action OnDisconnectEvent;

        public TileEngineClient(INetworkClient networkClient, string username)
        {
            _networkClient = networkClient;
            _username = username;

            _networkClient.OnConnectionAccepted += OnConnectionAccepted;
            _networkClient.OnDisconnect += OnDisconnect;
            _networkClient.OnDataReceived += OnDataReceived;

            TimeManager.Instance.NextFrameEvent += OnNextFrame;
        }

        public void Connect(float timeout)
        {
            _connectionTimeout = timeout;
            _connectionPending = true;
            _networkClient.Connect(_username, 0);
        }

        private void OnNextFrame()
        {
            if (_connectionTimeout > 0 && !_isConnected)
            {
                _connectionTimeout -= TimeManager.Instance.DeltaTime;
            }
            else if ((!_isConnected) && _connectionPending)
            {
                Console.WriteLine($"Is pending: {_connectionPending}");
                _connectionTimeout = 0;
                _connectionPending = false;
                OnConnectionTimeoutEvent?.Invoke();
            }

            _networkClient.Poll();
        }

        private void OnConnectionAccepted()
        {
            _isConnected = true;
            OnConnectionAcceptedEvent?.Invoke();
        }

        private void OnDisconnect()
        {
            _isConnected = false;
            OnDisconnectEvent?.Invoke();
        }

        private void OnDataReceived(byte[] data)
        {
            // TODO TileObjects, DialogForms and stuff
        }
    }
}
