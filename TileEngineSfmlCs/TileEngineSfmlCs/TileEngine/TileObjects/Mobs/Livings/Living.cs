using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Mobs.Livings
{
    /// <summary>
    /// Living mob has Death marker, that indicates, if this mob is alive or dead
    /// </summary>
    public abstract class Living : Mob
    {
        public abstract bool IsAlive { get; }
    }
}
