﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.TileEngine.SceneSerialization;
using TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public class EditorItem : Item
    {
        private float _power = 100;

        public override Icon Icon { get; } = new Icon("Images\\LaserGun.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\LaserGun.png");
        public override TileLayer Layer => TileLayer.Items;
        public override string VisibleName => "Laser gun";
        public override string ExamineDescription => "Deadly futuristic weapon";

        public float Power => _power;
       
        protected override void AppendUserFields(XmlElement baseElement)
        {
            SerializationUtils.Write(_power, nameof(_power), baseElement);
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            _power = SerializationUtils.ReadFloat(nameof(_power), baseElement, _power);
        }
    }
}
