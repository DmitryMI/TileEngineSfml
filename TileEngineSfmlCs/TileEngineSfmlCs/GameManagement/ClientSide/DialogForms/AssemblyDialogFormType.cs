using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public class AssemblyDialogFormType : DialogFormType
    {
        public AssemblyDialogFormType(Type assemblyType)
        {
            FormBaseType = assemblyType;
            Name = assemblyType.FullName;
        }

        public override string Name { get; }
        public override Type FormBaseType { get; }
        public override bool CanActivate => !FormBaseType.IsAbstract;
        public override DialogFormSpirit Activate()
        {
            object instance = Activator.CreateInstance(FormBaseType);
            return (DialogFormSpirit) instance;
        }
    }
}
