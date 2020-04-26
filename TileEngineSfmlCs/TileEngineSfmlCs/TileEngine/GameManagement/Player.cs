using System.Collections.Generic;
using TileEngineSfmlCs.TileEngine.GameManagement.DialogForms;
using TileEngineSfmlCs.TileEngine.GameManagement.Networking;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs;

namespace TileEngineSfmlCs.TileEngine.GameManagement
{
    public class Player
    {
        private int _connectionId;
        private Mob _controlledMob;
        private Camera _camera;
        private bool _isConnected;

        private List<IDialogForm> _dialogForms = new List<IDialogForm>();

        public List<IDialogForm> DialogForms => _dialogForms;

        public string Username { get; }

        public int ConnectionId => _connectionId;

        public bool IsConnected
        {
            get => _isConnected;
        }

        internal void SetConnected(bool connected)
        {
            _isConnected = connected;
        }

        public Mob ControlledMob
        {
            get => _controlledMob;
            set => _controlledMob = value;
        }
        public Camera Camera => _camera;

        public Player(int connectionId, string username, Camera camera, Mob controlledMob)
        {
            _controlledMob = controlledMob;
            _connectionId = connectionId;
            _camera = camera;
            _isConnected = true;
        }
    }
}
