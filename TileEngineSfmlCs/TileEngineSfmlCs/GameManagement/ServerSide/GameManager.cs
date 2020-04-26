using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using UdpNetworkInterface.UdpNetworkServer;

namespace TileEngineSfmlCs.GameManagement.ServerSide
{
    public class GameManager
    {
        #region Singleton

        private static GameManager _instance;

        public static GameManager Instance
        {
            get => _instance;
            set => _instance = value;
        }


        #endregion

        private Scene _scene;
        private ITimeProvider _timeProvider;
        private INetworkServer _networkServer;

        public void StartGame(Scene scene, ITimeProvider timeProvider, INetworkServer server)
        {
            _scene = scene;
            _networkServer = server;
            NetworkManager.Instance = new NetworkManager(server, _scene);
            _timeProvider = timeProvider;
            _timeProvider.NextFrameEvent += NextFrame;
            NetworkManager.Instance.OnPlayerConnected += OnPlayerConnected;
        }

        private void NextFrame()
        {
            NetworkManager.Instance.OnNextFrame();
            _scene.OnNextFrame();
        }

        private void OnPlayerConnected(Player player)
        {
            LobbyDialogForm lobbyDialog = new LobbyDialogForm();
            lobbyDialog.InteractingPlayer = player;
            NetworkManager.Instance.SpawnDialogForm(lobbyDialog);
        }
    }
}
