using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Walls
{
    public abstract class Wall : Turf
    {
        public override TileLayer Layer { get; } = TileLayer.Walls;
        public override bool IsPassable { get; } = false;
    }
}
