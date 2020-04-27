using System;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    /// <summary>
    /// Represents DialogForm on client side
    /// </summary>
    public abstract class DialogFormSpirit
    {
        public virtual DialogFormType GetDialogFormType()
        {
            throw new NotImplementedException();
        }
    }
}
