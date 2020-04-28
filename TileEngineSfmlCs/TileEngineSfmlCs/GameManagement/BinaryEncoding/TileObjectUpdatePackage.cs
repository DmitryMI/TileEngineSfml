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
        
        public int InstanceId { get; private set; }
        public Vector2Int Position { get; private set; }
        public Vector2 Offset { get; private set; }
        public TileLayer Layer { get; private set; }
        public int LayerOrder { get; private set; }
        public Icon Icon { get; private set; }
        public bool IsPassable { get; private set; }
        public bool IsLightTransparent { get; private set; }

        public TileObjectUpdatePackage(TileObject tileObject)
        {
            InstanceId = tileObject.GetInstanceId();
            Position = tileObject.Position;
            Offset = tileObject.Offset;
            Layer = tileObject.Layer;
            LayerOrder = tileObject.LayerOrder;
            Icon = tileObject.Icon;
            IsPassable = tileObject.IsPassable;
            IsLightTransparent = tileObject.IsLightTransparent;
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

            InstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);

            Position = new Vector2Int();
            Position.FromByteArray(data, pos);
            pos += Position.ByteLength;

            Offset = new Vector2();
            Offset.FromByteArray(data, pos);
            pos += Offset.ByteLength;

            Layer = (TileLayer) data[pos];
            pos++;
            LayerOrder = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);

            IsPassable = data[pos] == 1 ? true : false;
            pos++;
            IsLightTransparent = data[pos] == 1 ? true : false;
            pos++;

            Icon = new Icon();
            Icon.FromByteArray(data, pos);
            pos += Icon.ByteLength;
        }

        private void FillBuffer()
        {
            int length = sizeof(int) + Position.ByteLength + Offset.ByteLength + sizeof(byte) + sizeof(byte) + Icon.ByteLength;
            _buffer = new byte[length];

            int pos = 0;

            byte[] instanceIdBytes = BitConverter.GetBytes(InstanceId);
            Array.Copy(instanceIdBytes, 0, _buffer, pos, instanceIdBytes.Length);
            pos += instanceIdBytes.Length;

            Position.ToByteArray(_buffer, pos);
            pos += Position.ByteLength;
            Offset.ToByteArray(_buffer, pos);
            pos += Offset.ByteLength;

            _buffer[pos] = (byte)Layer;
            pos++;
            byte[] layerOrderBytes = BitConverter.GetBytes(LayerOrder);
            Array.Copy(layerOrderBytes, 0, _buffer, pos, layerOrderBytes.Length);
            pos += sizeof(int);

            byte isPassableByte =  (byte)(IsPassable ? 1 : 0);
            byte isLightTransparentByte = (byte)(IsLightTransparent ? 1 : 0);
            _buffer[pos] = isPassableByte;
            pos++;
            _buffer[pos] = isLightTransparentByte;
            pos++;
            
            Icon.ToByteArray(_buffer, pos);
            pos += Icon.ByteLength;
        }
    }
}
