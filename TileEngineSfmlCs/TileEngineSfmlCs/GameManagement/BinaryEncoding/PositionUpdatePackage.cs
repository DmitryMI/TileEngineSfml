using System;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct PositionUpdatePackage : IBinaryEncodable
    {
        public int InstanceId { get; set; }
        public Vector2Int Position { get; set; }
        public Vector2 Offset { get; set; }

        public PositionUpdatePackage(TileObject tileObject)
        {
            InstanceId = tileObject.GetInstanceId();
            Position = tileObject.Position;
            Offset = tileObject.Offset;
            ByteLength = sizeof(int) + Position.ByteLength + Offset.ByteLength;
        }

        public int ByteLength { get; private set; }
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            InstanceId = BitConverter.ToInt32(package, pos);
            pos += sizeof(int);
            Position.ToByteArray(package, pos);
            pos += Position.ByteLength;
            Offset.ToByteArray(package, pos);
            pos += Offset.ByteLength;
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            InstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            Position = new Vector2Int();
            Position.FromByteArray(data, pos);
            pos += Position.ByteLength;
            Offset = new Vector2();
            Offset.FromByteArray(data, pos);
            pos += Offset.ByteLength;
        }
    }
}