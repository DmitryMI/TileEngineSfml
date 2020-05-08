using System;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TypeManagement.EntityTypes
{
    public abstract class EntityType
    {
        public abstract string Name { get;}
        public abstract string FullName { get; }

        public Type BaseType { get; set; }

        public abstract bool CanActivate { get; }

        public abstract TileObject Activate();

        public abstract FieldDescriptor[] GetFieldDescriptors(bool ignoreTrash = true);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
            {
                return false;
            }

            EntityType entityType = (EntityType) obj;

            if (BaseType != entityType.BaseType)
            {
                return false;
            }

            if (Name != entityType.Name)
            {
                return false;
            }

            return true;
        }

        protected bool Equals(EntityType other)
        {
            return Equals(this, other);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
