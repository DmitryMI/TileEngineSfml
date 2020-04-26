using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects.Mobs;

namespace TileEngineSfmlCs.TileEngine.Networking
{
    public class Player
    {
        private int _connectionId;
        private Mob _controlledMob;
        private Camera _camera;

        public int ConnectionId => _connectionId;

        public Mob ControlledMob
        {
            get => _controlledMob;
            set => _controlledMob = value;
        }
        public Camera Camera => _camera;
    }
}
