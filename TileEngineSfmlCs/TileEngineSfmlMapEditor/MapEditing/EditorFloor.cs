using System.Xml;
using TileEngineSfmlCs.TileEngine.TileObjects.Turfs;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlMapEditor.MapEditing
{
    [TypeManagerIgnore(IgnoranceReason.TestObject)]
    public class EditorFloor : Turf
    {

        public override Icon Icon { get; } = new Icon("Images\\EditorFloor.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\EditorFloor.png");
        public override TileLayer Layer => TileLayer.Floor;
        public override string VisibleName => "Editor floor";
        public override string ExamineDescription => "This is an example floor tile for editor usage only";
        public override bool RequiresUpdates => false;
        public override bool IsPassable => true;
        public override bool IsLightTransparent => true;
        public override bool IsGasTransparent => true;

        protected override void AppendUserFields(XmlElement baseElement)
        {
            
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }

        
        public EditorFloor()
        {
            
        }
    }
}
