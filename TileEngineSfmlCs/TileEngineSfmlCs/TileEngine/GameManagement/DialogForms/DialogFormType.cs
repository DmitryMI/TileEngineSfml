using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.GameManagement.DialogForms
{
    public abstract class DialogFormType
    {
        public abstract string Name { get; }
        public abstract string FullName { get; }
        public abstract Type DialogBaseType { get; }
        public abstract IDialogForm Activate();
    }
}
