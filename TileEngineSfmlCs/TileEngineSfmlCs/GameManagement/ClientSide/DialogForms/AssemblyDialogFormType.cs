using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.GameManagement.ClientSide.DialogForms
{
    public class AssemblyDialogFormType : DialogFormType
    {
        public AssemblyDialogFormType(Type spiritType)
        {
            SpiritBaseType = spiritType;
            SpiritName = spiritType.FullName;
        }

        public override string SpiritName { get; }
        public override string FormName { get; }
        public override Type SpiritBaseType { get; }
        public override Type FormBaseType { get; }
        public override bool CanActivate => !SpiritBaseType.IsAbstract;


        public override DialogFormSpirit ActivateSpirit(int instanceId)
        {
            object instance = Activator.CreateInstance(SpiritBaseType, instanceId );
            return (DialogFormSpirit) instance;
        }
    }
}
