using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.TileEngine.SceneSerialization;

namespace TileEngineSfmlCs.Types
{

    [Obsolete]
    public struct SpriteSettings : IFieldSerializer
    {
        public int ResourceId { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 CenterOffset { get; set; }
        public float Rotation { get; set; }
        public ColorB Color { get; set; }

        public int LayerId { get; set; }
        public int LayerIndex { get; set; }

        public void AppendFields(XmlElement parentElement)
        {
            if (parentElement.OwnerDocument == null)
            {
                return;
            }

            SerializationUtils.Write(ResourceId, nameof(ResourceId), parentElement);
            SerializationUtils.Write(Scale, nameof(Scale), parentElement);
            SerializationUtils.Write(CenterOffset, nameof(CenterOffset), parentElement);
            SerializationUtils.Write(Rotation, nameof(Rotation), parentElement);
            SerializationUtils.Write(LayerId, nameof(LayerId), parentElement);
            SerializationUtils.Write(LayerIndex, nameof(LayerIndex), parentElement);
            SerializationUtils.Write(Color, nameof(Color), parentElement);
        }

        public void ReadFields(XmlElement element)
        {
            ResourceId = SerializationUtils.ReadInt(nameof(ResourceId), element);
            Scale = (Vector2)SerializationUtils.ReadFieldSerializer(nameof(Scale), Scale, element);
            CenterOffset = (Vector2)SerializationUtils.ReadFieldSerializer(nameof(CenterOffset), CenterOffset, element);
            Rotation = SerializationUtils.ReadFloat(nameof(Rotation), element);
            LayerId = SerializationUtils.ReadInt(nameof(LayerId), element);
            LayerIndex = SerializationUtils.ReadInt(nameof(LayerIndex), element);
            Color = (ColorB)SerializationUtils.ReadFieldSerializer(nameof(Color), Color, element);
        }
    }
}
