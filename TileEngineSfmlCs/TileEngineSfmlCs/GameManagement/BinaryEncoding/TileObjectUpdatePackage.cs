using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.TileObjects;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public class TileObjectUpdatePackage : IBinaryEncodable
    {
        private byte[] _buffer;
        private Vector2Int _position;
        private Vector2 _offset;

        public int InstanceId { get; private set; }

        public Vector2Int Position
        {
            get => _position;
            private set => _position = value;
        }

        public Vector2 Offset
        {
            get => _offset;
            private set => _offset = value;
        }

        public TileLayer Layer { get; private set; }
        public int LayerOrder { get; private set; }
        public bool IsPassable { get; private set; }
        public bool IsLightTransparent { get; private set; }
        public string VisibleName { get; private set; }
        public Icon Icon { get; private set; }

        public TileObjectUpdatePackage(TileObject tileObject)
        {
            InstanceId = tileObject.GetInstanceId();
            _position = tileObject.Position;
            _offset = tileObject.Offset;
            Layer = tileObject.Layer;
            LayerOrder = tileObject.LayerOrder;
            Icon = tileObject.Icon;
            IsPassable = tileObject.IsPassable;
            IsLightTransparent = tileObject.IsLightTransparent;
            VisibleName = tileObject.VisibleName;
            FillBuffer();
        }

        public TileObjectUpdatePackage()
        {
            _buffer = new Byte[0];
        }

        public int ByteLength => _buffer.Length;
        public int ToByteArray(byte[] package, int index)
        {
            Array.Copy(_buffer, 0, package, index, _buffer.Length);
            return _buffer.Length;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;

            // InstanceId
            InstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);

            // _position
            _position = new Vector2Int();
            _position.FromByteArray(data, pos);
            pos += _position.ByteLength;

            // _offset
            _offset = new Vector2();
            _offset.FromByteArray(data, pos);
            pos += _offset.ByteLength;

            // Layer
            Layer = (TileLayer) data[pos];
            pos++;

            // LayerOrder
            LayerOrder = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);

            // IsPassable
            IsPassable = data[pos] == 1 ? true : false;
            pos++;

            // IsLightTransparent
            IsLightTransparent = data[pos] == 1 ? true : false;
            pos++;

            // VisibleName
            int nameLength = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            VisibleName = Encoding.Unicode.GetString(data, pos, nameLength);
            pos += nameLength;

            // Icon
            Icon = new Icon();
            Icon.FromByteArray(data, pos);
            pos += Icon.ByteLength;
        }

        private void FillBuffer()
        {
            byte[] nameBytes = Encoding.Unicode.GetBytes(VisibleName);
            byte[] nameLengthBytes = BitConverter.GetBytes(nameBytes.Length);

            int length =
                sizeof(int) + // InstanceId
                _position.ByteLength + // _position
                _offset.ByteLength + // _offset
                sizeof(byte) + // Layer
                sizeof(int) + // LayerOrder
                sizeof(byte) + // IsPassable
                sizeof(byte) + // IsLightTransparent
                sizeof(int) + // Length of VisibleName
                nameBytes.Length; // VisibleName
            if (Icon == null)
            {
                Icon = new Icon();
            }

            length += Icon.ByteLength;  // Icon

            _buffer = new byte[length];

            int pos = 0;

            // InstanceId
            byte[] instanceIdBytes = BitConverter.GetBytes(InstanceId);
            Array.Copy(instanceIdBytes, 0, _buffer, pos, instanceIdBytes.Length);
            pos += instanceIdBytes.Length;

            // _position
            _position.ToByteArray(_buffer, pos);
            pos += _position.ByteLength;

            // _offset
            _offset.ToByteArray(_buffer, pos);
            pos += _offset.ByteLength;

            // Layer
            _buffer[pos] = (byte)Layer;
            pos++;

            // LayerOrder
            byte[] layerOrderBytes = BitConverter.GetBytes(LayerOrder);
            Array.Copy(layerOrderBytes, 0, _buffer, pos, layerOrderBytes.Length);
            pos += sizeof(int);

            // IsPassable
            byte isPassableByte =  (byte)(IsPassable ? 1 : 0);
            _buffer[pos] = isPassableByte;
            pos++;

            // IsLightTransparent
            byte isLightTransparentByte = (byte)(IsLightTransparent ? 1 : 0);
            _buffer[pos] = isLightTransparentByte;
            pos++;

            // VisibleName
            Array.Copy(nameLengthBytes, 0, _buffer, pos, nameLengthBytes.Length);
            pos += nameLengthBytes.Length;
            Array.Copy(nameBytes, 0, _buffer, pos, nameBytes.Length);
            pos += nameBytes.Length;

            // Icon
            Icon.ToByteArray(_buffer, pos);
            pos += Icon.ByteLength;
        }
    }
}
