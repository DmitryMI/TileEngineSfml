using System;
using System.Xml;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.Scripting.PythonObjects
{
    [TypeManagerIgnore(IgnoranceReason.NotReallyAnObject)]
    public class PythonObject : TileObject
    {
        public override Icon Icon { get; }
        public override Icon EditorIcon { get; }
        public override TileLayer Layer { get; }
        public override string VisibleName { get; }
        public override string ExamineDescription { get; }
        public override bool RequiresUpdates { get; }
        public override bool IsPassable { get; }
        public override bool IsLightTransparent { get; }
        public override bool IsGasTransparent { get; }
        protected override void AppendUserFields(XmlElement baseElement)
        {
            throw new NotImplementedException();
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            throw new NotImplementedException();
        }
    }
}
