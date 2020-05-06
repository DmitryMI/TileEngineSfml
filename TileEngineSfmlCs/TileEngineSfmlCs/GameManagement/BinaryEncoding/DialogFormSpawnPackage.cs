using System;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct DialogFormSpawnPackage : IBinaryEncodable
    {
        public int InstanceId { get; set; }
        public int TypeIndex { get; set; }
        public int ByteLength => sizeof(int) + sizeof(int);

        public DialogFormSpawnPackage(int instanceId, int typeIndex)
        {
            InstanceId = instanceId;
            TypeIndex = typeIndex;
        }

      
        public int ToByteArray(byte[] package, int index)
        {
            byte[] instanceIdBytes = BitConverter.GetBytes(InstanceId);
            byte[] typeIndexBytes = BitConverter.GetBytes(TypeIndex);
            int pos = index;
            Array.Copy(instanceIdBytes, 0, package, pos, instanceIdBytes.Length);
            pos += sizeof(int);
            Array.Copy(typeIndexBytes, 0, package, pos, typeIndexBytes.Length);
            pos += sizeof(int);
            return ByteLength;
        }

        public void FromByteArray(byte[] payload, int index)
        {
            int pos = index;
            InstanceId = BitConverter.ToInt32(payload, pos);
            pos += sizeof(int);
            TypeIndex = BitConverter.ToInt32(payload, pos);
            pos += sizeof(int);
        }
    }
}