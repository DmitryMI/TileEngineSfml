using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ResourcesManager;
using TileEngineSfmlCs.TileEngine.SceneSerialization;

namespace TileEngineSfmlCs.Types
{
    public class Icon : IFieldSerializer
    {
        private List<int> _resourceIds = new List<int>();
        private List<ColorB> _colors = new List<ColorB>();

        public int SpritesCount => _resourceIds.Count;

        public Icon(params string[] images)
        {
            for (int i = 0; i < images.Length; i++)
            {
                int resourceId = GameResources.Instance.GetResourceId(images[i]);
                _resourceIds.Add(resourceId);
                _colors.Add(ColorB.White);
            }
        }

        public void AppendFields(XmlElement parentElement)
        {
            SerializationUtils.WriteParseables(_resourceIds.ToArray(), nameof(_resourceIds), parentElement);
            SerializationUtils.WriteParseables(_resourceIds.ToArray(), nameof(_colors), parentElement);
        }

        public void ReadFields(XmlElement parentElement)
        {
            int[] resources = SerializationUtils.ReadParseables<int>(nameof(_resourceIds), parentElement);
            _resourceIds.AddRange(resources);

            SerializationUtils.ReadFieldSerializers<ColorB>(nameof(_colors), parentElement);
        }

        public int[] ResourceIds => _resourceIds.ToArray();
        public ColorB[] Colors => _colors.ToArray();

        public int GetResourceId(int orderIndex) => _resourceIds[orderIndex];
        public ColorB GetColor(int orderIndex) => _colors[orderIndex];
    }
}
