using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileEngineSfmlCs.TileEngine.SceneSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.Types;

// ReSharper disable once CheckNamespace
namespace TileEngineSfmlCs.TileEngine.SceneSerialization.Tests
{
    [TestClass()]
    public class SerializerTests
    {
        private Random _random = new Random();

        class SampleTileObject : TileObject
        {
            private float _healthPoints;
            private Vector2 _vector2;
            private SpriteSettings _defaultSprite;
            private SpriteSettings _altSprite;
            private int ActiveSpriteIndex { get; set; }
        

            public override SpriteSettings[] SpritesSettings
            {
                get
                {
                    if (ActiveSpriteIndex == 0)
                    {
                        return new[] {_defaultSprite};
                    }
                    else
                    {
                        return new[] { _altSprite };
                    }
                }
                
            }
            
            public override bool IsPassable { get; }
            public override bool IsLightTransparent { get;  }
            public override bool IsGasTransparent { get;  }
            protected override void ReadUserFields(XmlElement baseElement)
            {
                _healthPoints = SerializationUtils.ReadFloat(nameof(_healthPoints), baseElement);
                _vector2 = SerializationUtils.ReadFieldSerializer<Vector2>(nameof(_vector2), baseElement);
            }

            protected override void AppendUserFields(XmlElement baseElement)
            {
                SerializationUtils.Write(_healthPoints, nameof(_healthPoints), baseElement);
                SerializationUtils.Write(_vector2, nameof(_vector2), baseElement);
            }

            public SampleTileObject()
            {

            }

            public SampleTileObject(int health, Vector2 vector2, SpriteSettings defaultSprite, SpriteSettings altSprite)
            {
                _healthPoints = health;
                _vector2 = vector2;
                
            }
        }

        private TileObject CreateRandomObject()
        {
            SpriteSettings defaultSprite = new SpriteSettings()
            {
                ResourceId = 1,
                Color = new ColorB(255, 255, 255 ,50)
            };
            SpriteSettings altSprite = new SpriteSettings()
            {
                ResourceId = 2,
                Color = new ColorB(255, 255, 255, 100)
            };
            Vector2 offset = new Vector2((float) _random.NextDouble(), (float) _random.NextDouble());
            TileObject sampleObject = new SampleTileObject(_random.Next(0, 100), offset, defaultSprite, altSprite);
            sampleObject.Position = new Vector2Int(_random.Next(1, 10), _random.Next(1, 10));
            sampleObject.Offset = new Vector2((float)_random.NextDouble(), (float)_random.NextDouble());
            return sampleObject;
        }

        private string GetRandomSerializedScene()
        {
            TileObject sampleObjectA = CreateRandomObject();
            TileObject sampleObjectB = CreateRandomObject();
            Scene scene = new Scene(10, 10);
            scene.Instantiate(sampleObjectA);
            scene.Instantiate(sampleObjectB);
            MemoryStream stream = new MemoryStream();
            Serializer.SerializeScene(scene, stream);

            string xml = Encoding.UTF8.GetString(stream.ToArray());

            return xml;
        }

        [TestMethod()]
        public void SerializeSceneTest()
        {
            string xml = GetRandomSerializedScene();

            Debug.WriteLine(xml);
        }

        [TestMethod()]
        public void DeserializeSceneTest()
        {
            string xml = GetRandomSerializedScene();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;

            Scene scene = Serializer.DeserializeScene(stream);

            Debug.WriteLine(scene.Width + ", " + scene.Height);
        }
    }
}