using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct SceneInformationPackage : IBinaryEncodable
    {
        public Vector2Int Size { get; set; }
        public int ByteLength => Size.ByteLength;
        public int ToByteArray(byte[] package, int index)
        {
            Size.ToByteArray(package, index);
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            Size = new Vector2Int();
            Size.FromByteArray(data, index);
        }

        public SceneInformationPackage(Scene scene)
        {
            Size = new Vector2Int(scene.Width, scene.Height);
        }

    }
}