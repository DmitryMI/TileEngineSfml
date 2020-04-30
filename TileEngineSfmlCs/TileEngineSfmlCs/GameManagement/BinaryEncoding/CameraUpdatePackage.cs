using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Types;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct CameraUpdatePackage : IBinaryEncodable
    {
        private int _trackingInstanceId;
        private Vector2Int _cameraCenter;
        private Vector2Int _viewportSize;

        public int TrackingInstanceId
        {
            get => _trackingInstanceId;
            set => _trackingInstanceId = value;
        }

        public Vector2Int CameraCenter
        {
            get => _cameraCenter;
            set => _cameraCenter = value;
        }

        public Vector2Int ViewportSize
        {
            get => _viewportSize;
            set => _viewportSize = value;
        }

        public int ByteLength => sizeof(int) + CameraCenter.ByteLength + ViewportSize.ByteLength;

        public CameraUpdatePackage(Camera camera)
        {
            _cameraCenter = camera.Center;
            _viewportSize = camera.Size;

            if (camera.TrackingTarget != null)
            {
                _trackingInstanceId = camera.TrackingTarget.InstanceId;
            }
            else
            {
                _trackingInstanceId = -1;
            }
        }

        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            byte[] trackingInstanceIdBytes = BitConverter.GetBytes(_trackingInstanceId);
            Array.Copy(trackingInstanceIdBytes, 0, package, pos, trackingInstanceIdBytes.Length);
            pos += trackingInstanceIdBytes.Length;
            _cameraCenter.ToByteArray(package, pos);
            pos += _cameraCenter.ByteLength;
            _viewportSize.ToByteArray(package, pos);
            pos += _viewportSize.ByteLength;
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            _trackingInstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            _cameraCenter.FromByteArray(data, pos);
            pos += _cameraCenter.ByteLength;
            _viewportSize.FromByteArray(data, pos);
            pos += _viewportSize.ByteLength;
        }
    }
}
