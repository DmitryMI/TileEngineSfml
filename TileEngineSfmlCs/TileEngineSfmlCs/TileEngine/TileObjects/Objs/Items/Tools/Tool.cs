using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items.Tools
{
    public abstract class Tool : Item
    {
        public abstract ToolBehaviour Behaviour { get; }
    }
}
