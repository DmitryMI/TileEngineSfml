using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.TypeManagement
{
    public abstract class FieldDescriptor
    {
        public abstract string Name { get; }
        public abstract bool IsRuntimeOnly { get; }
        public virtual string RuntimeOnlyMessage => "[Runtime Only]";
        public abstract bool IsReadOnly { get; }
        public virtual string ReadOnlyMessage => "[Read Only]";
        public abstract void SetValue(object instance, object value);
        public abstract object GetValue(object instance);

        public static bool IsRuntimeType(Type type)
        {
            bool isValueType = type.IsValueType;
            if (isValueType)
                return false;
            bool isTileObject = typeof(TileObjects.TileObject).IsAssignableFrom(type);
            return !isTileObject;
        }

        public static bool IsReadOnlyField(FieldInfo fieldInfo)
        {
            CustomAttributeData customAttributeData = fieldInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(FieldEditorReadOnlyAttribute));
            if (customAttributeData != null)
            {
                return true;
            }

            return false;
        }
    }
}
