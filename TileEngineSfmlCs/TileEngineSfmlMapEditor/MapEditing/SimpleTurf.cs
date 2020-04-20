using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlMapEditor.MapEditing
{
    public class SimpleTurf : TileObject
    {
        private SpriteSettings[] _layerSettings;

        public override SpriteSettings[] SpritesSettings => _layerSettings;

        public override bool IsPassable => true;
        public override bool IsLightTransparent => true;
        public override bool IsGasTransparent => true;

        protected override void AppendUserFields(XmlElement baseElement)
        {
            
        }

        protected override void ReadUserFields(XmlElement baseElement)
        {
            
        }

        
        public SimpleTurf()
        {
            var settings = new SpriteSettings[1];
            SpriteSettings sprite = new SpriteSettings();
            sprite.Scale = new Vector2(1, 1);
            sprite.Color = new ColorB(255, 255, 255, 255);
            sprite.ResourceId = ResourcesManager.GameResources.Instance.GetResourceId("Images\\SimpleTurf.png");
            settings[0] = sprite;
            _layerSettings = settings;
        }
    }
}
