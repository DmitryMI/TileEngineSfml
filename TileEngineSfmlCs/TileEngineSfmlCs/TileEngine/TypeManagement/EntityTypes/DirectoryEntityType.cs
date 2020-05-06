using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes
{
    public class DirectoryEntityType : EntityType
    {
        public DirectoryEntityType(string name)
        {
            Name = name;
        }
        public override string Name { get; }
        public override string FullName => Name;
        public override bool CanActivate => false;

        public override TileObject Activate()
        {
            throw new InvalidOperationException();
        }

        public override FieldDescriptor[] GetFieldDescriptors(bool ignoreTrash = true)
        {
            throw new InvalidOperationException();
        }
    }
}
