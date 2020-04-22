using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.TypeManagement
{
    public class FieldEditorReadOnlyAttribute : Attribute
    {
        public string Message { get; }
        public FieldEditorReadOnlyAttribute(string message)
        {
            Message = message;
        }
    }
}
