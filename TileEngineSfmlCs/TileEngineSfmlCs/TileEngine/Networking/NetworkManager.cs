using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdpNetworkInterface.UdpNetworkServer;

namespace TileEngineSfmlCs.TileEngine.Networking
{
    public class NetworkManager
    {
        #region Singleton

        private static NetworkManager _instance;

        public static NetworkManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        private INetworkServer _networkServer;
        private List<Player> _players = new List<Player>();

        public NetworkManager(INetworkServer networkServer)
        {
            _networkServer = networkServer;
            _networkServer.OnDataReceived += OnDataReceived;
            _networkServer.OnNewConnection += OnNewConnection;
            _networkServer.OnDisconnect += OnDisconnect;
            _networkServer.OnReconnect += OnReconnect;
        }

        private void OnNewConnection(int connectionId)
        {

        }

        private void OnDisconnect(int connectionId)
        {

        }

        private void OnReconnect(int connectionId)
        {

        }

        private void OnDataReceived(int connectionId, byte[] data)
        {

        }

        public void OnNextFrame()
        {

        }
    }
}
