using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Types;
using UdpNetworkInterface.UdpNetworkClient;

namespace TileEngineSfmlCs.GameManagement.ClientSide
{
    public class ClientNetworkManager
    {
        #region Singleton

        private static ClientNetworkManager _instace;

        public static ClientNetworkManager Instance
        {
            get => _instace;
            set => _instace = value;
        }
        #endregion

        private INetworkClient _networkClient;
        private string _username;
        private bool _isConnected;
        private bool _connectionPending;
        private float _connectionTimeout;

        public bool IsConnected => _isConnected;

        public event Action OnConnectionAcceptedEvent;
        public event Action OnConnectionTimeoutEvent;
        public event Action OnDisconnectEvent;

        public ClientNetworkManager(INetworkClient networkClient, string username)
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
            // Dialog forms
            NetworkAction action = (NetworkAction) data[0];
            Debug.WriteLine("NetworkAction: " + action.ToString());
            byte[] payload = new byte[data.Length - 1];
            Array.Copy(data, 1, payload, 0, payload.Length);
            int payloadPos = 0;
            
            switch (action)
            {
                case NetworkAction.TileObjectUpdate:
                    // TODO Update tile object
                    break;
                case NetworkAction.DialogFormUpdate:
                    DialogFormUpdatePackage updateDialog = new DialogFormUpdatePackage();
                    updateDialog.FromByteArray(payload, payloadPos);
                    var spirit = DialogFormManager.Instance.GetFormSpirit(updateDialog.InstanceId);
                    spirit.OnServerDataUpdate(updateDialog.Key, updateDialog.Input);
                    break;
                case NetworkAction.TileObjectSpawn:
                    break;
                case NetworkAction.DialogFormSpawn:
                    Debug.WriteLine("Spawning dialog form");
                    DialogFormSpawnPackage package = new DialogFormSpawnPackage();
                    package.FromByteArray(payload, payloadPos);
                    DialogFormType formType = DialogFormManager.Instance.GetTypeByIndex(package.TypeIndex);
                    DialogFormManager.Instance.CreateDialogFormSpirit(formType, package.InstanceId);
                    break;
                case NetworkAction.ControlInput:
                    // Server cannot send control inputs to the player
                    break;
                case NetworkAction.DialogFormInput:
                    // Server cannot send dialog inputs to the player
                    break;
                case NetworkAction.DialogFormServerClose:
                    Debug.WriteLine("Dialog close command received");
                    DialogFormServerClosePackage dialogFormServerClosePackage = new DialogFormServerClosePackage();
                    dialogFormServerClosePackage.FromByteArray(payload, payloadPos);
                    DialogFormManager.Instance.KillDialogFormSpirit(dialogFormServerClosePackage.InstanceId);
                    break;
                case NetworkAction.DialogFormUserClose:
                    break;
                default:
                    Debug.WriteLine("Unknown command");
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SendDialogFormInput(DialogFormSpirit spirit, string key, string input)
        {
            int instanceId = spirit.InstanceId;
            DialogFormInputPackage package = new DialogFormInputPackage(instanceId, key, input);
            byte[] data = new byte[1 + package.ByteLength];
            int pos = 0;
            data[pos] = (byte)NetworkAction.DialogFormInput;
            pos += 1;
            package.ToByteArray(data, pos);
            _networkClient.Send(data);
        }
    }
}
