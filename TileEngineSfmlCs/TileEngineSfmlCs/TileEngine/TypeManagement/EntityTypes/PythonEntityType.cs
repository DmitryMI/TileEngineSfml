using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes
{
    public class PythonEntityType : EntityType
    {
        public override string Name { get; }
        public override bool CanActivate { get; }
        public override TileObject Activate()
        {
            throw new NotImplementedException();
        }

        public override FieldDescriptor[] GetFieldDescriptors(bool ignoreTrash = true)
        {
            throw new NotImplementedException();
        }
    }
}
