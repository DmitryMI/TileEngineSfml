using System.Xml;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TileObjects.Turfs;
using TileEngineSfmlCs.TypeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlMapEditor.MapEditing
{
    [TypeManagerIgnore(IgnoranceReason.TestObject)]
    public class EditorWall : Turf
    {
        public override Icon Icon { get; } = new Icon("Images\\EditorWall.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\EditorWall.png");
        public override void TryPass(TileObject sender)
        {
            
        }

        public override TileLayer Layer => TileLayer.Walls;
        public override string VisibleName => "Editor wall";

        public override string ExamineDescription =>
            "It is an example wall for editor testing. If you see this inside the game, then forget about it";

        public override bool RequiresUpdates => false;
        public override bool IsPassable => false;
        public override bool IsLightTransparent => false;
        public override bool IsGasTransparent => false;
        protected override void AppendUserFields(XmlElement baseElement)
        {
           
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }
    }
}
