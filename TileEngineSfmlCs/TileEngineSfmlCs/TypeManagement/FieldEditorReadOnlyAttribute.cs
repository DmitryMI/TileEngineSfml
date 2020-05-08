using System;

namespace TileEngineSfmlCs.TypeManagement
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
