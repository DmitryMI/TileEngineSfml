using System;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public abstract class DialogFormType
    {
        public abstract string Name { get; }

        public abstract Type FormBaseType { get; }

        public abstract bool CanActivate { get; }

        public abstract DialogFormSpirit Activate();
    }
}
