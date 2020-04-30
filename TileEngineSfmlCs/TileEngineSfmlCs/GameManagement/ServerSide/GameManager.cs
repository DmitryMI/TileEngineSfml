using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms.Lobby;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Networking;
using TileEngineSfmlCs.Networking.UdpNetworkServer;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs.Livings.Carbons.Mammals;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Types;
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
            _scene?.OnNextFrame();
        }

        private void OnPlayerConnected(Player player)
        {
            LogManager.RuntimeLogger.Log($"Player {player.Username}({player.ConnectionId}) connected.");
            LobbyDialogForm lobbyDialog = new LobbyDialogForm();
            lobbyDialog.InteractingPlayer = player;
            lobbyDialog.PlayerJoinCallback = PlayerJoinCallback;
            DialogFormManager.Instance.AssignFormIndex(lobbyDialog);
            NetworkManager.Instance.SpawnDialogForm(lobbyDialog);
        }
        

        private void PlayerJoinCallback(LobbyDialogForm sender)
        {
            Player player = sender.InteractingPlayer;
            NetworkManager.Instance.KillDialogForm(sender);
            LogManager.RuntimeLogger.Log($"Player {player.Username}({player.ConnectionId}) joined the game!");

            Corgi corgi = new Corgi();
            // TODO Get player spawn position!
            corgi.Position = new Vector2Int(_scene.Width / 2, _scene.Height / 2);
            corgi.SetDogName(sender.FirstName + " " + sender.LastName);

            _scene.Instantiate(corgi);

            player.ControlledMob = corgi;
            player.Camera.TrackingTarget = corgi;

            NetworkManager.Instance.UpdateScene(player);
            NetworkManager.Instance.UpdateCamera(player, Reliability.Reliable);
        }
    }
}
