using System;
using System.Collections.Generic;
using System.Xml;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Utils.Serialization;
using GameResources = TileEngineSfmlCs.TileEngine.ResourceManagement.GameResources;

namespace TileEngineSfmlCs.Types
{
    public class Icon : IFieldSerializer, IBinaryEncodable
    {
        private List<int> _resourceIds;
        private List<ColorB> _colors;
        private List<float> _scales;

        public void Clear()
        {
            _resourceIds.Clear();
            _colors.Clear();
            _scales.Clear();
        }

        public int SpritesCount => _resourceIds.Count;

        public Icon()
        {
            _resourceIds = new List<int>();
            _colors = new List<ColorB>();
            _scales = new List<float>();
        }

        public Icon(params string[] images)
        {
            _resourceIds = new List<int>();
            _colors = new List<ColorB>();
            _scales = new List<float>();
            for (int i = 0; i < images.Length; i++)
            {
                int resourceId = GameResources.Instance.GetResourceId(images[i]);
                if (resourceId == -1)
                {
                    LogManager.EditorLogger.LogError($"Resource {images[i]} not found!");
                    LogManager.RuntimeLogger.LogError($"Resource {images[i]} not found!");
                }
                _resourceIds.Add(resourceId);
                _colors.Add(ColorB.White);
                _scales.Add(1);
            }
        }

        public void AppendFields(XmlElement parentElement)
        {
            SerializationUtils.WriteParseables(_resourceIds.ToArray(), nameof(_resourceIds), parentElement);
            SerializationUtils.WriteParseables(_resourceIds.ToArray(), nameof(_colors), parentElement);
            SerializationUtils.WriteParseables(_scales.ToArray(), nameof(_scales), parentElement);
        }

        public void ReadFields(XmlElement parentElement)
        {
            _resourceIds.Clear();
            _colors.Clear();
            _scales.Clear();
            int[] resources = SerializationUtils.ReadParseables<int>(nameof(_resourceIds), parentElement, null);
            _resourceIds.AddRange(resources);

            float[] scales = SerializationUtils.ReadParseables<float>(nameof(_scales), parentElement, null);
            _scales.AddRange(scales);

            SerializationUtils.ReadFieldSerializers<ColorB>(nameof(_colors), parentElement, null);
        }

        public int[] ResourceIds => _resourceIds.ToArray();
        public ColorB[] Colors => _colors.ToArray();
        public float[] Scales => _scales.ToArray();

        public void AddSprite(string image, ColorB color, float scale)
        {
            int resourceId = GameResources.Instance.GetResourceId(image);
            _resourceIds.Add(resourceId);
            _colors.Add(color);
            _scales.Add(scale);
        }

        public void RemoveSpriteByIndex(int index)
        {
            _resourceIds.RemoveAt(index);
            _colors.RemoveAt(index);
            _scales.RemoveAt(index);
        }

        public void RemoveSprite(int resourceId)
        {
            int index = _resourceIds.IndexOf(resourceId);
            RemoveSpriteByIndex(index);
        }

        public void RemoveSprite(string image)
        {
            int resourceId = GameResources.Instance.GetResourceId(image);
            RemoveSprite(resourceId);
        }

        public int GetResourceId(int orderIndex) => _resourceIds[orderIndex];
        public ColorB GetColor(int orderIndex) => _colors[orderIndex];
        public float GetScale(int orderIndex) => _scales[orderIndex];

        public void SetColor(int orderIndex, ColorB color) => _colors[orderIndex] = color;
        public void GetScale(int orderIndex, float scale) => _scales[orderIndex] = scale;
        public int ByteLength => sizeof(int) + SpritesCount * (sizeof(int) + ColorB.White.ByteLength + sizeof(float));
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            byte[] spritesCountLength = BitConverter.GetBytes(SpritesCount);
            Array.Copy(spritesCountLength, 0, package, pos, spritesCountLength.Length);
            pos += spritesCountLength.Length;

            for (int i = 0; i < SpritesCount; i++)
            {
                byte[] idBytes = BitConverter.GetBytes(_resourceIds[i]);
                Array.Copy(idBytes, 0, package, pos, idBytes.Length);
                pos += idBytes.Length;
               
                _colors[i].ToByteArray(package, pos);
                pos += _colors[i].ByteLength;

                byte[] scaleBytes = BitConverter.GetBytes(_scales[i]);
                Array.Copy(scaleBytes, 0, package, pos, scaleBytes.Length);
                pos += scaleBytes.Length;
            }

            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            _resourceIds.Clear();
            _colors.Clear();
            _scales.Clear();
            int pos = index;
            int spritesCount = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            for (int i = 0; i < spritesCount; i++)
            {
                int id = BitConverter.ToInt32(data, pos);
                pos += sizeof(int);
                ColorB color = new ColorB();
                color.FromByteArray(data, pos);
                pos += color.ByteLength;
                float scale = BitConverter.ToSingle(data, pos);
                pos += sizeof(float);

                _resourceIds.Add(id);
                _colors.Add(color);
                _scales.Add(scale);
            }
        }
    }
}
