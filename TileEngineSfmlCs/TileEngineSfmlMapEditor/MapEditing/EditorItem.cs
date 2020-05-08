using System.Xml;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.TileEngine.TileObjects.Objs.Items;
using TileEngineSfmlCs.TypeManagement;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlMapEditor.MapEditing
{
    [TypeManagerIgnore(IgnoranceReason.TestObject)]
    public class EditorItem : Item
    {
        private float _power = 100;

        public override Icon Icon { get; } = new Icon("Images\\LaserGun.png");
        public override Icon EditorIcon { get; } = new Icon("Images\\LaserGun.png");
        public override void TryPass(TileObject sender)
        {
            
        }

        public override TileLayer Layer => TileLayer.Items;
        public override string VisibleName => "Laser gun";
        public override string ExamineDescription => "Deadly futuristic weapon";
        public override bool RequiresUpdates => false;

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
