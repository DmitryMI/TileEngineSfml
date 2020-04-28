using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Networking.UdpNetworkServer;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Utils;

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
            DialogFormManager.Instance = new DialogFormManager();
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
            DialogFormManager.Instance.AssignFormIndex(lobbyDialog);
            NetworkManager.Instance.SpawnDialogForm(lobbyDialog);

            DelayedAction delayedAction = new DelayedAction(TimeManager.Instance);
            delayedAction.Delay(CloseLobbyForm, lobbyDialog, 60);
        }

        private void CloseLobbyForm(DelayedAction delayedAction, object argument)
        {
            LobbyDialogForm lobbyDialog = (LobbyDialogForm) argument;
            NetworkManager.Instance.KillDialogForm(lobbyDialog);
            DialogFormManager.Instance.UnregisterFormIndex(lobbyDialog);
            LogManager.RuntimeLogger.Log($"[GameManager] Lobby killed for player {lobbyDialog.InteractingPlayer.Username}({lobbyDialog.InteractingPlayer.ConnectionId})");
        }
    }
}
