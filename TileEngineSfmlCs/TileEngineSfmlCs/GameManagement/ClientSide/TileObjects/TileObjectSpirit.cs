using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.ClientSide.TileObjects
{
    public class TileObjectSpirit : IPositionProvider
    {
        private Icon _icon;
        private Vector2Int _position;
        private Vector2 _offset;

        public int InstanceId { get; set; }

        public bool LocationDirty { get; set; }

        public Vector2Int Position
        {
            get => _position;
            set
            {
                LocationDirty = true;
                _position = value;
            }
        }

        public Vector2 Offset
        {
            get => _offset;
            set
            {
                LocationDirty = true;
                _offset = value;
            }
        }
        public TileLayer Layer { get; set; }
        public int LayerOrder { get; set; }
        public bool IsPassable { get; set; }
        public bool IsLightTransparent { get; set; }

        public string VisibleName { get; set; }

        public bool IconDirty { get; set; }

        /// <summary>
        /// User data associated with this Spirit
        /// </summary>
        public object Tag { get; set; }

        public Icon Icon
        {
            get => _icon;
            set
            {
                IconDirty = true;
                _icon = value;
            }
        }
       
    }
}
