using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;

namespace TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes
{
    public class AssemblyEntityType : EntityType
    {
        public class AssemblyFieldDescriptor : FieldDescriptor
        {
            private readonly System.Reflection.FieldInfo _fieldInfo;

            public override string ReadOnlyMessage
            {
                get
                {
                    CustomAttributeData attributeData = _fieldInfo.CustomAttributes.FirstOrDefault(a =>
                        a.AttributeType == typeof(FieldEditorReadOnlyAttribute));
                    if (attributeData != null)
                    {
                        var attribute = (FieldEditorReadOnlyAttribute)Attribute.GetCustomAttribute(_fieldInfo, typeof(FieldEditorReadOnlyAttribute));
                        return attribute.Message;
                    }
                    else
                    {
                        return base.ReadOnlyMessage;
                    }
                }
            }

            public AssemblyFieldDescriptor(FieldInfo fieldInfo)
            {
                _fieldInfo = fieldInfo;
            }

            public override string Name => _fieldInfo.Name;
            public override bool IsRuntimeOnly => IsRuntimeType(_fieldInfo.FieldType);
            public override bool IsReadOnly => IsReadOnlyField(_fieldInfo);
            public override void SetValue(object instance, object value)
            {
                if (IsRuntimeOnly)
                {
                    throw new RuntimeFieldAccessRestrictedException();
                }
                _fieldInfo.SetValue(instance, value);
            }

            public override object GetValue(object instance)
            {
                return _fieldInfo.GetValue(instance);
            }
        }

        public AssemblyEntityType(Type type)
        {
            BaseType = type;
        }

        private bool CheckActivate()
        {
            bool notNull = BaseType != null;
            if (!notNull)
                return false;

            bool notAbstract = !BaseType.IsAbstract;
            bool derives = typeof(TileObject).IsAssignableFrom(BaseType);

            if (notAbstract && derives)
            {
                return true;
            }

            return false;
        }

        public override string Name => BaseType.Name;
        public override bool CanActivate => CheckActivate();


        public override TileObject Activate()
        {
            return (TileObject) Activator.CreateInstance(BaseType);
        }

        public override FieldDescriptor[] GetFieldDescriptors(bool ignoreTrash = true)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            fieldInfos.AddRange(BaseType.GetFields(flags));

            Type currentType = BaseType.BaseType;
            while (currentType != null && currentType != typeof(Object))
            {
                var infos = currentType.GetFields(flags);
                foreach (var fieldInfo in infos)
                {
                    fieldInfos.Add(fieldInfo);
                }

                currentType = currentType.BaseType;
            }

            List<FieldDescriptor> descriptors = new List<FieldDescriptor>(fieldInfos.Count);
            for (var i = 0; i < fieldInfos.Count; i++)
            {
                var fieldInfo = fieldInfos[i];
                if (ignoreTrash && fieldInfo.Name.Contains("BackingField"))
                {
                    continue;
                }
                
                descriptors.Add(new AssemblyFieldDescriptor(fieldInfo));
            }

            return descriptors.ToArray();
        }
    }
}
