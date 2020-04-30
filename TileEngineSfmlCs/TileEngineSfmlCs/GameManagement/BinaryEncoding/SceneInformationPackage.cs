using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct SceneInformationPackage : IBinaryEncodable
    {
        private Vector2Int _size;

        public Vector2Int Size
        {
            get => _size;
            set => _size = value;
        }

        public int ByteLength => Size.ByteLength;
        public int ToByteArray(byte[] package, int index)
        {
            Size.ToByteArray(package, index);
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            _size = new Vector2Int(50, 50);
            _size.FromByteArray(data, index);
        }

        public SceneInformationPackage(Scene scene)
        {
            _size = new Vector2Int(scene.Width, scene.Height);
        }

    }
}