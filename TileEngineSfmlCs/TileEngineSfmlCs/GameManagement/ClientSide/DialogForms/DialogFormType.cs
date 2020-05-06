using System;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public abstract class DialogFormType
    {
        public abstract string SpiritName { get; }
        public abstract string FormName { get; }

        public abstract Type SpiritBaseType { get; }
        public abstract Type FormBaseType { get; }

        public abstract bool CanActivate { get; }

        public abstract DialogFormSpirit ActivateSpirit(int instanceId);
    }
}
