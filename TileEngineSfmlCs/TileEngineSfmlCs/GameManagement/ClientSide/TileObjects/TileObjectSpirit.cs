using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.ClientSide.TileObjects
{
    public class TileObjectSpirit
    {
        public Vector2Int Position { get; set; }
        public Vector2 Offset { get; set; }
        public Icon Icon { get; set; }
        public TileLayer Layer { get; set; }
        public int LayerOrder { get; set; }
        public bool IsPassable { get; set; }
        public bool IsLightTransparent { get; set; }

    }
}
