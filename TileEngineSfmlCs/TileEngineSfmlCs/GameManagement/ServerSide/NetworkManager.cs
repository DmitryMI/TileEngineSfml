using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.GameManagement.ServerSide.DialogForms;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;
using UdpNetworkInterface.UdpNetworkServer;

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

        public NetworkManager(INetworkServer networkServer, Scene controlledScene)
        {
            _networkServer = networkServer;
            _controlledScene = controlledScene;
            _networkServer.OnDataReceived += OnDataReceived;
            _networkServer.OnNewConnection += OnNewConnection;
            _networkServer.OnDisconnect += OnDisconnect;
            _networkServer.OnReconnect += OnReconnect;

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

        private void SendDataToDialog(Player player, int dialogInstanceId, byte[] data)
        {
            IDialogForm dialogForm = player.DialogForms.FirstOrDefault(d => d.DialogInstanceId == dialogInstanceId);
            if (dialogForm == null)
            {
                return;
            }

            int pos = 0;
            int keySize = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            int inputSize = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            string key = Encoding.Unicode.GetString(data, pos, keySize);
            pos += keySize;
            string input = Encoding.Unicode.GetString(data, pos, inputSize);
            pos += inputSize;

            dialogForm.OnUserInput(key, input);
        }


        private void OnDataReceived(int connectionId, byte[] packageData)
        {
            Player player = GetPlayerByConnection(connectionId);
            // TODO Player data received
            NetworkAction action = (NetworkAction) packageData[0];
            switch (action)
            {
                case NetworkAction.DialogFormInput:
                    int dialogInstanceId = packageData[1];
                    byte[] dialogData = new byte[packageData.Length - 2];
                    Array.Copy(packageData, 2, dialogData, 0, packageData.Length - 2);
                    SendDataToDialog(player, dialogInstanceId, dialogData);
                    break;
                case NetworkAction.ControlInput:
                    // TODO User control input
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

        public void UpdateTileObject(TileObject tileObject)
        {
            // TODO UpdateTileObject
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
            _networkServer.SendData(dialogForm.InteractingPlayer.ConnectionId, data);
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
            _networkServer.SendData(dialogForm.InteractingPlayer.ConnectionId, data);
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
            _networkServer.SendData(dialogForm.InteractingPlayer.ConnectionId, data);
        }

    }
}
