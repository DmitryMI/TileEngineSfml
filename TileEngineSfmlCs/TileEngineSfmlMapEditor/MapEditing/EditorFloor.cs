using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TileObjects.Turfs;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public class EditorFloor : Turf
    {

        public override Icon Icon { get; } = new Icon("Images\\EditorFloor.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\EditorFloor.png");
        public override TileLayer Layer => TileLayer.Floor;
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
