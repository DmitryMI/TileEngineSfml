using System.Xml;

namespace TileEngineSfmlCs.Utils.Serialization
{
    public interface IFieldSerializer
    {
        void AppendFields(XmlElement parentElement);
        void ReadFields(XmlElement parentElement);
    }
}