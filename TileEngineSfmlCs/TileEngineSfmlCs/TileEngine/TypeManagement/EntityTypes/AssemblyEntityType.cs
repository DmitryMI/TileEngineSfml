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
        public AssemblyEntityType(Type type)
        {
            BaseType = type;
        }

        private bool CheckActivate()
        {
            bool notNull = BaseType != null;
            if (!notNull)
                return false;

            bool notAbstract = !BaseType.IsAbstract;
            bool derives = typeof(TileObject).IsAssignableFrom(BaseType);

            if (notAbstract && derives)
            {
                return true;
            }

            return false;
        }

        public override string Name => BaseType.Name;
        public override bool CanActivate => CheckActivate();


        public override TileObject Activate()
        {
            return (TileObject) Activator.CreateInstance(BaseType);
        }
    }
}
