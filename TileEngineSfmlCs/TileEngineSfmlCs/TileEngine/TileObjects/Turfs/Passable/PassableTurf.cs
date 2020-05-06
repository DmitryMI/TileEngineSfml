using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Passable
{
    public abstract class PassableTurf : Turf
    {
        public override bool IsPassable => true;
        public override bool IsLightTransparent => true;
        public override bool IsGasTransparent => true;
        
        public abstract SoundClip[] FootstepClips { get; }
    }
}
