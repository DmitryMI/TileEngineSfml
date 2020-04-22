using System.Xml;
using TileEngineSfmlCs.TileEngine.SceneSerialization;

namespace TileEngineSfmlCs.Types
{
    public struct ColorB : IFieldSerializer
    {
        public static ColorB White { get; } = new ColorB(255, 255, 255, 255);

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public byte A { get; set; }

        public ColorB(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public ColorB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public void AppendFields(XmlElement parentElement)
        {
            SerializationUtils.Write(R, nameof(R), parentElement);
            SerializationUtils.Write(G, nameof(G), parentElement);
            SerializationUtils.Write(B, nameof(B), parentElement);
            SerializationUtils.Write(A, nameof(A), parentElement);
        }

        public void ReadFields(XmlElement parentElement)
        {
            R = (byte)SerializationUtils.ReadInt(nameof(R), parentElement);
            G = (byte)SerializationUtils.ReadInt(nameof(G), parentElement);
            B = (byte)SerializationUtils.ReadInt(nameof(B), parentElement);
            A = (byte)SerializationUtils.ReadInt(nameof(A), parentElement);
        }
    }
}