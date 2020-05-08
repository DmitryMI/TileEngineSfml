using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.GameManagement.BinaryEncoding.ControlInput;
using TileEngineSfmlCs.GameManagement.DialogForms;
using TileEngineSfmlCs.GameManagement.SoundManagement;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Networking;
using TileEngineSfmlCs.Networking.UdpNetworkServer;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.ServerSide
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

        #region Events

        public event Action<Player> OnPlayerConnected;
        public event Action<Player> OnPlayerDisconnected; 

        #endregion

        private INetworkServer _networkServer;
        private List<Player> _players = new List<Player>();
        private Scene _controlledScene;

        public Player[] GetPlayers() => _players.ToArray();

        public NetworkManager(INetworkServer networkServer, Scene controlledScene)
        {
            _networkServer = networkServer;
            _controlledScene = controlledScene;
            _networkServer.OnDataReceived += OnDataReceived;
            _networkServer.OnNewConnection += OnNewConnection;
            _networkServer.OnDisconnect += OnDisconnect;
            _networkServer.OnReconnect += OnReconnect;
            _networkServer.NewConnectionResponse = NewConnectionResponse;

            networkServer.StartServer();
        }

        public Player GetPlayerByConnection(int connectionId)
        {
            foreach (var p in _players)
            {
                if (p.ConnectionId == connectionId)
                {
                    return p;
                }
            }

            return null;
        }

        private void OnNewConnection(int connectionId, string username)
        {
            Vector2Int center = new Vector2Int(_controlledScene.Width / 2, _controlledScene.Height / 2);
            Camera camera = new Camera(center, new Vector2Int(20, 20));
            Player player = new Player(connectionId, username, camera, null);
            _players.Add(player);

            OnPlayerConnected?.Invoke(player);
        }

        private byte[] NewConnectionResponse()
        {
            SceneInformationPackage informationPackage = new SceneInformationPackage(_controlledScene);
            byte[] data = new byte[informationPackage.ByteLength];
            informationPackage.ToByteArray(data, 0);
            return data;
        }

        private void OnDisconnect(int connectionId)
        {
            Player player = GetPlayerByConnection(connectionId);
            if (player == null)
            {
                // TODO Player not spawned
                return;
            }

            player.SetConnected(false);
            OnPlayerDisconnected?.Invoke(player);
        }

        private void OnReconnect(int connectionId)
        {
            Player player = GetPlayerByConnection(connectionId);
            if (player == null)
            {
                // TODO Player not spawned
                return;
            }

            player.SetConnected(true);
            // TODO Player reconnected sequence
        }


        private void OnDataReceived(int connectionId, byte[] packageData)
        {
            Player player = GetPlayerByConnection(connectionId);
            // TODO Player data received
            NetworkAction action = (NetworkAction) packageData[0];
            Debug.WriteLine($"Action: {action}");
            switch (action)
            {
                case NetworkAction.DialogFormInput:
                    DialogFormInputPackage inputPackage = new DialogFormInputPackage();
                    inputPackage.FromByteArray(packageData, 1);
                    IDialogForm dialogForm = player.DialogForms.FirstOrDefault(d => d.DialogInstanceId == inputPackage.InstanceId);
                    if (dialogForm == null)
                    {
                        LogManager.RuntimeLogger.LogError(
                            $"Player {connectionId} does not own dialog form with id {inputPackage.InstanceId}");
                        return;
                    }

                    dialogForm.OnUserInput(inputPackage.Key, inputPackage.Input);
                    break;
                case NetworkAction.ControlInput:
                    ControlInputPackage controlInput = new ControlInputPackage();
                    controlInput.FromByteArray(packageData, 1);
                    // TODO Clicks
                    //LogManager.RuntimeLogger.Log("Input received. Movement: " + controlInput.MovementKey);
                    player?.ControlledMob?.Move(controlInput.MovementKey);
                    break;
                default:
                    // Player in not eligible to perform other network actions
                    break;
            }
        }

        public void OnNextFrame()
        {
            _networkServer.Poll();
        }

        public void UpdateTileObject(TileObject tileObject, Reliability reliability = Reliability.Reliable)
        {
            if (tileObject.GetInstanceId() == -1)
            {
                // TileObject is not instantiated
                return;
            }
            TileObjectUpdatePackage wrapper = new TileObjectUpdatePackage(tileObject);
            byte[] data = new byte[1 + wrapper.ByteLength];
            int pos = 0;
            data[pos] = (byte) NetworkAction.TileObjectUpdate;
            pos += 1;
            wrapper.ToByteArray(data, pos);

            foreach (var player in _players)
            {
                _networkServer.SendData(player.ConnectionId, data, reliability);
            }
        }

        public void UpdateCamera(Player player, Reliability reliability = Reliability.Reliable)
        {
            CameraUpdatePackage updatePackage = new CameraUpdatePackage(player.Camera);
            byte[] data = new byte[1 + updatePackage.ByteLength];
            int pos = 0;
            data[pos] = (byte)NetworkAction.CameraUpdate;
            pos += 1;
            updatePackage.ToByteArray(data, pos);
            _networkServer.SendData(player.ConnectionId, data, reliability);
        }

        public void UpdatePosition(TileObject tileObject, Reliability reliability = Reliability.Unreliable)
        {
            if (tileObject.GetInstanceId() == -1)
            {
                // TileObject is not instantiated
                return;
            }

            PositionUpdatePackage wrapper = new PositionUpdatePackage(tileObject);
            byte[] data = new byte[1 + wrapper.ByteLength];
            int pos = 0;
            data[pos] = (byte)NetworkAction.TileObjectUpdate;
            pos += 1;
            wrapper.ToByteArray(data, pos);

            foreach (var player in _players)
            {
                _networkServer.SendData(player.ConnectionId, data, reliability);
            }
        }

        public void DestroyTileObject(TileObject tileObject, Reliability reliability = Reliability.Reliable)
        {
            // TODO Destroy tile object
        }

        public void SpawnTileObject(TileObject tileObject)
        {
            // TODO SpawnTileObject
        }

        public void SpawnDialogForm(IDialogForm dialogForm)
        {
            int instanceId = dialogForm.DialogInstanceId;
            DialogFormType spiritType = dialogForm.SpiritType;
            int typeIndex = DialogFormManager.Instance.GetTypeIndex(spiritType);

            DialogFormSpawnPackage spawnDialogPackage = new DialogFormSpawnPackage(instanceId, typeIndex);
            byte[] data = new byte[1 + spawnDialogPackage.ByteLength];
            int pos = 0;
            data[pos] = (byte)NetworkAction.DialogFormSpawn;
            pos += 1;
            spawnDialogPackage.ToByteArray(data, pos);
            Player player = dialogForm.InteractingPlayer;
            if (!player.DialogForms.Contains(dialogForm))
            {
                player.DialogForms.Add(dialogForm);
            }
            _networkServer.SendData(dialogForm.InteractingPlayer.ConnectionId, data, Reliability.Reliable);
        }

        public void KillDialogForm(IDialogForm dialogForm)
        {
            int instanceId = dialogForm.DialogInstanceId;
            DialogFormServerClosePackage package = new DialogFormServerClosePackage(instanceId);
            byte[] data = new byte[1 + package.ByteLength];
            int pos = 0;
            data[pos] = (byte) NetworkAction.DialogFormServerClose;
            pos += 1;
            package.ToByteArray(data, pos);
            _networkServer.SendData(dialogForm.InteractingPlayer.ConnectionId, data, Reliability.Reliable);
        }

        public void UpdateDialogForm(IDialogForm dialogForm, string key, string input)
        {
            int instanceId = dialogForm.DialogInstanceId;
            DialogFormUpdatePackage package = new DialogFormUpdatePackage(instanceId, key, input);
            byte[] data = new byte[1 + package.ByteLength];
            int pos = 0;
            data[pos] = (byte)NetworkAction.DialogFormUpdate;
            pos += 1;
            package.ToByteArray(data, pos);
            _networkServer.SendData(dialogForm.InteractingPlayer.ConnectionId, data, Reliability.Reliable);
        }

        public void UpdateScene(Player player)
        {
            foreach (var tileObject in _controlledScene.TileObjects)
            {
                if (tileObject.GetInstanceId() == -1)
                {
                    return;
                }

                TileObjectUpdatePackage wrapper = new TileObjectUpdatePackage(tileObject);
                byte[] data = new byte[1 + wrapper.ByteLength];
                int pos = 0;
                data[pos] = (byte) NetworkAction.TileObjectUpdate;
                pos += 1;
                wrapper.ToByteArray(data, pos);

                _networkServer.SendData(player.ConnectionId, data, Reliability.Reliable);
            }
        }

        public void UpdateSound(SoundClipInstance clipInstance, IEnumerable<Player> players, Reliability reliability)
        {
            byte[] data = new byte[1 + clipInstance.ByteLength];
            int pos = 0;
            data[pos] = (byte)NetworkAction.SoundUpdate;
            pos += 1;
            clipInstance.ToByteArray(data, pos);

            foreach (var player in players)
            {
                _networkServer.SendData(player.ConnectionId, data, reliability);
            }
        }

    }
}
