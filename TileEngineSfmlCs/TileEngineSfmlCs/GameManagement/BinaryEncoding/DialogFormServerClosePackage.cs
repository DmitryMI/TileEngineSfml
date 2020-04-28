using System;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct DialogFormServerClosePackage : IBinaryEncodable
    {
        public int InstanceId { get; private set; }
        public int ByteLength => sizeof(int);

        public DialogFormServerClosePackage(int instanceId)
        {
            InstanceId = instanceId;
        }

        public int ToByteArray(byte[] package, int index)
        {
            byte[] instanceIdBytes = BitConverter.GetBytes(InstanceId);
            Array.Copy(instanceIdBytes, 0, package, index, instanceIdBytes.Length);
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            InstanceId = BitConverter.ToInt32(data, index);
        }
    }
}