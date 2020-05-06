using System;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct PositionUpdatePackage : IBinaryEncodable
    {
        private Vector2Int _position;
        private Vector2 _offset;
        public int InstanceId { get; set; }

        public Vector2Int Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        public PositionUpdatePackage(TileObject tileObject)
        {
            InstanceId = tileObject.GetInstanceId();
            _position = tileObject.Position;
            _offset = tileObject.Offset;
            ByteLength = sizeof(int) + _position.ByteLength + _offset.ByteLength;
        }

        public int ByteLength { get; private set; }
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            InstanceId = BitConverter.ToInt32(package, pos);
            pos += sizeof(int);
            _position.ToByteArray(package, pos);
            pos += Position.ByteLength;
            _offset.ToByteArray(package, pos);
            pos += Offset.ByteLength;
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            InstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            _position = new Vector2Int();
            _position.FromByteArray(data, pos);
            pos += _position.ByteLength;
            _offset = new Vector2();
            _offset.FromByteArray(data, pos);
            pos += _offset.ByteLength;
        }
    }
}