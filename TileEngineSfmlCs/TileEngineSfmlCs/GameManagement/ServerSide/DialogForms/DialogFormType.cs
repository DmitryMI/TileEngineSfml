using System;

namespace TileEngineSfmlCs.GameManagement.ServerSide.DialogForms
{
    public abstract class DialogFormType
    {
        public abstract string Name { get; }
        public abstract string FullName { get; }
        public abstract Type DialogBaseType { get; }
        public abstract IDialogForm Activate();
    }
}
