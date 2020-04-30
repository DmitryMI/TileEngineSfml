using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.TileEngine.TileObjects.Turfs.Floors
{
    public class MetalFloor : Floor
    {
        public override Icon Icon { get; } = new Icon("Images\\Floors\\MetalFloor.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\Floors\\MetalFloor.png");
        public override string VisibleName { get; } = "Metal floor";
        public override string ExamineDescription { get; } = "Metal floor that covers wires and ventilation";
        public override bool RequiresUpdates { get; } = false;

        protected override void AppendUserFields(XmlElement baseElement)
        {
            
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }
    }
}
