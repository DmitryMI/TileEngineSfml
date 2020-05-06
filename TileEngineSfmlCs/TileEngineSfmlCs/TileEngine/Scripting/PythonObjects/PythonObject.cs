using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.TileEngine.TypeManagement.EntityTypes;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.TileEngine.Scripting.PythonObjects
{
    [TypeManagerIgnore(IgnoranceReason.NotReallyAnObject)]
    public class PythonObject : TileObject
    {
        private string _sourceCode;
        private PythonEntityType _entityType;
        private readonly ScriptEngine _scriptEngine;
        private ScriptRuntime _scriptRuntime;
        private ScriptScope _scriptScope;
        private string[] _variablesNames;
        private Icon _icon;

        public string SourceCode => _sourceCode;

        public ScriptRuntime ScriptRuntime => _scriptRuntime;
        public ScriptScope ScriptScope => _scriptScope;

        public PythonEntityType EntityType => _entityType;

        private void UpdateIcon()
        {
            _icon.Clear();
            string iconPath = _scriptScope.GetVariable("icon");
            _icon.AddSprite(iconPath, ColorB.White, 1);
        }

        public PythonObject(Stream pythonSourceCodeStream, PythonEntityType entityType)
        {
            _entityType = entityType;
            byte[] codeBytes = new byte[pythonSourceCodeStream.Length];
            pythonSourceCodeStream.Read(codeBytes, 0, codeBytes.Length);
            _sourceCode = Encoding.UTF8.GetString(codeBytes);

            _scriptEngine = Python.CreateEngine();

            FromSourceCode(_sourceCode);

            pythonSourceCodeStream.Close();
        }

        public PythonObject(string sourceCode, PythonEntityType entityType)
        {
            _entityType = entityType;
            _scriptEngine = Python.CreateEngine();
            FromSourceCode(sourceCode);
        }

        private void FromSourceCode(string code)
        {
            _sourceCode = code;
            _scriptScope = _scriptEngine.CreateScope();
            _scriptRuntime = _scriptEngine.Runtime;
            _scriptEngine.Execute(_sourceCode, _scriptScope);

            
            _variablesNames = _scriptScope.GetVariableNames().ToArray();
            string imagePath = _scriptScope.GetVariable("icon");
            Debug.WriteLine($"[PythonObject] Loading image {imagePath}");
            _icon = new Icon(imagePath);
            if (_icon.ResourceIds[0] == -1)
            {
                LogManager.EditorLogger.LogError($"Image {imagePath} was not found!");
            }
        }

        public void SetValue(string variableName, object value)
        {
            _scriptScope.SetVariable(variableName, value);
        }

        public object GetValue(string variableName)
        {
            return _scriptScope.GetVariable(variableName);
        }

        public override Icon Icon => _icon;
        public override Icon EditorIcon => new Icon(_scriptScope.GetVariable("editor_icon"));
        public override void TryPass(TileObject sender)
        {
            // TODO Transmit to script
        }

        public override TileLayer Layer =>
            Enum.Parse(typeof(TileLayer), _scriptScope.GetVariable("layer"), true);

        public override string VisibleName => _scriptScope.GetVariable("name");
        public override string ExamineDescription => _scriptScope.GetVariable("description");
        public override bool RequiresUpdates => _scriptScope.GetVariable<bool>("requires_updates");
        public override bool IsPassable => _scriptScope.GetVariable<bool>("is_passable");
        public override bool IsLightTransparent => _scriptScope.GetVariable<bool>("is_light_transparent");
        public override bool IsGasTransparent => _scriptScope.GetVariable<bool>("is_gas_transparent");

        protected override void AppendUserFields(XmlElement baseElement)
        {
            foreach (var variableItem in _scriptScope.GetItems())
            {
                if (variableItem.Key.StartsWith("__") && variableItem.Key.EndsWith("__"))
                {
                    continue;
                }

                SerializationUtils.WriteParseable(variableItem.Value, variableItem.Key, baseElement);
            }
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            foreach (var variableItem in _scriptScope.GetItems())
            {
                if (variableItem.Key.StartsWith("__") && variableItem.Key.EndsWith("__"))
                    continue;
                object value  = SerializationUtils.ReadParseable(variableItem.Key, baseElement, variableItem.Value);
                _scriptScope.SetVariable(variableItem.Key, value);
            }
        }

        public override EntityType GetEntityType()
        {
            return _entityType;
        }
    }
}
