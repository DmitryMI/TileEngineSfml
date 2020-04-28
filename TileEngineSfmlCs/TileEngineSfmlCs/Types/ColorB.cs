using System.Xml;
using TileEngineSfmlCs.GameManagement.BinaryEncoding;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs.Types
{
    public struct ColorB : IFieldSerializer, IBinaryEncodable
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
            R = (byte)SerializationUtils.ReadInt(nameof(R), parentElement, R);
            G = (byte)SerializationUtils.ReadInt(nameof(G), parentElement, G);
            B = (byte)SerializationUtils.ReadInt(nameof(B), parentElement, B);
            A = (byte)SerializationUtils.ReadInt(nameof(A), parentElement, A);
        }

        public int ByteLength => 3;
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            package[pos] = R;
            pos++;
            package[pos] = G;
            pos++;
            package[pos] = B;
            pos++;
            return 3;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            R = data[pos];
            pos++;
            G = data[pos];
            pos++;
            B = data[pos];
            pos++;
        }
    }
}