using System;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes
{
    public abstract class EntityType
    {
        public abstract string Name { get;}

        public Type BaseType { get; set; }

        public abstract bool CanActivate { get; }

        public abstract TileObject Activate();
    }
}
