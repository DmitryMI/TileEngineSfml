using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.Networking
{
    public class Camera
    {
        public TileObject TrackingTarget { get; set; }
        public Vector2Int Center { get; set; }
        public Vector2Int Size { get; set; }
    }
}
