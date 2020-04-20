using System.Xml;

namespace TileEngineSfmlCs.TileEngine.SceneSerialization
{
    public interface IFieldSerializer
    {
        void AppendFields(XmlElement parentElement);
        void ReadFields(XmlElement parentElement);
    }
}