using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public class EditorItem : Item
    {
        public override Icon Icon { get; } = new Icon("Images\\LaserGun.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\LaserGun.png");
        public override TileLayer Layer => TileLayer.Items;
        public override string VisibleName => "Laser gun";
        public override string ExamineDescription => "Deadly futuristic weapon";
       
        protected override void AppendUserFields(XmlElement baseElement)
        {
            
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }
    }
}
