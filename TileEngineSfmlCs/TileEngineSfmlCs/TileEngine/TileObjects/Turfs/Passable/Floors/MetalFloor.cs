using System.Xml;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Passable.Floors
{
    public class MetalFloor : Floor
    {
        public override Icon Icon { get; } = new Icon("Images\\Floors\\MetalFloor.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\Floors\\MetalFloor.png");
        public override void TryPass(TileObject sender)
        {
            
        }

        public override string VisibleName { get; } = "Metal floor";
        public override string ExamineDescription { get; } = "Metal floor that covers wires and ventilation";
        public override bool RequiresUpdates { get; } = false;

        protected override void AppendUserFields(XmlElement baseElement)
        {
            
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }

        public override SoundClip[] FootstepClips { get; } = new[]
        {
            new SoundClip("Sounds\\Footsteps\\MetalFloor\\floor1.ogg"),
            new SoundClip("Sounds\\Footsteps\\MetalFloor\\floor2.ogg"),
            new SoundClip("Sounds\\Footsteps\\MetalFloor\\floor3.ogg"),
            new SoundClip("Sounds\\Footsteps\\MetalFloor\\floor4.ogg"),
            new SoundClip("Sounds\\Footsteps\\MetalFloor\\floor5.ogg"),
        };
    }
}
