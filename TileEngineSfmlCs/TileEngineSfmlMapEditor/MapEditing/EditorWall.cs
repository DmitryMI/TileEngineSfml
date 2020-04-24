using System.Xml;
using TileEngineSfmlCs.TileEngine.TileObjects.Turfs;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public class EditorWall : Turf
    {
        public override Icon Icon { get; } = new Icon("Images\\EditorWall.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\EditorWall.png");
        public override TileLayer Layer => TileLayer.Walls;
        public override string VisibleName => "Editor wall";

        public override string ExamineDescription =>
            "It is an example wall for editor testing. If you see this inside the game, then forget about it";
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
