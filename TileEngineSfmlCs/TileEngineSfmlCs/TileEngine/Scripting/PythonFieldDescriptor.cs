using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
using TileEngineSfmlCs.TileEngine.Scripting.PythonObjects;
using TileEngineSfmlCs.TypeManagement;
using TileEngineSfmlCs.Utils;

namespace TileEngineSfmlCs.TileEngine.Scripting
{
    public class PythonFieldDescriptor : FieldDescriptor
    {
        private PythonObject _prefabPythonObject;
        private string _fieldName;
        private bool _isParseable;

        public PythonFieldDescriptor(PythonObject prefabInstance, string fieldName)
        {
            _prefabPythonObject = prefabInstance;
            _fieldName = fieldName;
            _isParseable = CheckIfParseable();
        }

        public override string Name => _fieldName;
        public override bool IsRuntimeOnly => false;
        public override bool IsReadOnly => false;

        public override void SetValue(object instance, object value)
        {
            PythonObject pythonObject = (PythonObject) instance;
            pythonObject.SetValue(_fieldName, value);
        }

        public override object GetValue(object instance)
        {
            PythonObject pythonObject = (PythonObject) instance;
            return pythonObject.GetValue(_fieldName);
        }

        private bool CheckIfParseable()
        {
            ScriptScope scope = _prefabPythonObject.ScriptScope;
            Type type = scope.GetVariable(_fieldName).GetType();
            bool parseable = ParsingUtils.IsTextParseable(type);
            return parseable;
        }

        public override bool IsStringParseable => _isParseable;

        public override void ParseAndSet(object instance, string text)
        {
            Type type = _prefabPythonObject.ScriptScope.GetVariable(_fieldName).GetType();
            if (ParsingUtils.IsTextParseable(type))
            {
                object value = ParsingUtils.Parse(type, text);
                SetValue(instance, value);
            }
            else
            {
                // TODO Throw exception
            }
        }
    }
}

