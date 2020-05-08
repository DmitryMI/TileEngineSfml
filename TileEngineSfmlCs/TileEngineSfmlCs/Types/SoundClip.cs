using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.ResourceManagement;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.Types
{
    public struct SoundClip : IFieldSerializer, IBinaryEncodable
    {
        public int ResourceId { get; private set; }

        public SoundClip(int resourceId)
        {
            ResourceId = resourceId;
        }

        public SoundClip(string resourcePath)
        {
            int resourceId = GameResources.Instance.GetResourceId(resourcePath);
            if (resourceId == -1)
            {
                LogManager.EditorLogger.LogError($"[SoundClip] Resource {resourcePath} not found!");
                LogManager.RuntimeLogger.LogError($"[SoundClip] Resource {resourcePath} not found!");
            }

            ResourceId = resourceId;
        }

        public void AppendFields(XmlElement parentElement)
        {
            SerializationUtils.Write(ResourceId, nameof(ResourceId), parentElement);
        }

        public void ReadFields(XmlElement parentElement)
        {
            ResourceId = SerializationUtils.ReadInt(nameof(ResourceId), parentElement, ResourceId);
        }

        public int ByteLength => sizeof(int);

        public int ToByteArray(byte[] package, int index)
        {
            byte[] resourceIdBytes = BitConverter.GetBytes(ResourceId);
            Array.Copy(resourceIdBytes, 0, package, index, resourceIdBytes.Length);
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            ResourceId = BitConverter.ToInt32(data, index);
        }
    }
}
