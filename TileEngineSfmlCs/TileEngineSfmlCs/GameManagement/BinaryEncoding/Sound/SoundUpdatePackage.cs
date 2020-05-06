using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding.Sound
{
    public struct SoundUpdatePackage : IBinaryEncodable
    {
        public SoundClip Sound { get; set; }

        private int GetLength()
        {
            return Sound.ByteLength;
        }

        public int ByteLength => GetLength();
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            pos += Sound.ToByteArray(package, pos);
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            Sound.FromByteArray(data, pos);
            pos += Sound.ByteLength;
        }
    }
}