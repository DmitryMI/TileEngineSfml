using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.Logging;
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
        private INetworkServer _networkServer;

        public void StartGame(Scene scene, INetworkServer server)
        {
            _scene = scene;
            _networkServer = server;
            NetworkManager.Instance = new NetworkManager(server, _scene);
            TimeManager.Instance.NextFrameEvent += NextFrame;
            NetworkManager.Instance.OnPlayerConnected += OnPlayerConnected;
            LogManager.RuntimeLogger.Log($"Game started");
        }

        private void NextFrame()
        {
            NetworkManager.Instance.OnNextFrame();
            _scene.OnNextFrame();
        }

        private void OnPlayerConnected(Player player)
        {
            LogManager.RuntimeLogger.Log($"Player {player.Username}({player.ConnectionId}) connected.");
            LobbyDialogForm lobbyDialog = new LobbyDialogForm();
            lobbyDialog.InteractingPlayer = player;
            NetworkManager.Instance.SpawnDialogForm(lobbyDialog);
        }
    }
}
