using System;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using TileEngineSfmlCs.TileEngine.Scripting.PythonObjects;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TypeManagement;
using TileEngineSfmlCs.TypeManagement.EntityTypes;

namespace TileEngineSfmlCs.TileEngine.Scripting
{
    public class PythonEntityType : EntityType
    {
        private PythonObject _prefabInstance;
        private string _name;

        private string _filePath;

        public string FilePath => _filePath;

        public string SourceCode => _prefabInstance.SourceCode;

        public ScriptScope ScriptScope => _prefabInstance.ScriptScope;

        public PythonEntityType(PythonObject prefabInstance, string name, string filePath)
        {
            _prefabInstance = prefabInstance;
            _name = name;
            _filePath = filePath;
        }

        public PythonEntityType(PythonObject prefabInstance, string filePath)
        {
            _prefabInstance = prefabInstance;
            _name = prefabInstance.VisibleName;
            _filePath = filePath;
        }

        public override string Name => _name;
        public override string FullName => typeof(PythonObject).Name + "\\" + _filePath;
        public override bool CanActivate => true;
        public override TileObject Activate()
        {
            PythonObject instance = new PythonObject(_prefabInstance.SourceCode, this);
            return instance;
        }

        public override FieldDescriptor[] GetFieldDescriptors(bool ignoreTrash = true)
        {
            ScriptScope scope = _prefabInstance.ScriptScope;

            List<FieldDescriptor> result = new List<FieldDescriptor>();

            foreach (var variableItem in scope.GetItems())
            {
                if(variableItem.Key.StartsWith("_") && variableItem.Key.EndsWith("_"))
                    continue;
                
                PythonFieldDescriptor descriptor = new PythonFieldDescriptor(_prefabInstance, variableItem.Key);
                result.Add(descriptor);
            }

            return result.ToArray();
        }
    }
}
