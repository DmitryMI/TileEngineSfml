using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes
{
    public class AssemblyEntityType : EntityType
    {
        public override TileObject Activate()
        {
            return (TileObject) Activator.CreateInstance(BaseType);
        }
    }
}
