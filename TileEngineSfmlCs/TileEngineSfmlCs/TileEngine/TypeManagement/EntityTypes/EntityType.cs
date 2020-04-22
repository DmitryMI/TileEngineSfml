using System;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes
{
    public abstract class EntityType
    {
        public string Name { get; set; }

        public Type BaseType { get; set; }

        public abstract TileObject Activate();
    }
}
