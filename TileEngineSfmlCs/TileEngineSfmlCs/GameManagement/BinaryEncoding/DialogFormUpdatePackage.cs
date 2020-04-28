using System;
using System.Text;
using TileEngineSfmlCs.GameManagement.ClientSide.DialogForms;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding
{
    public struct DialogFormUpdatePackage : IBinaryEncodable
    {
        private byte[] _keyBytes;
        private byte[] _inputBytes;
        public int InstanceId { get; set; }
        public int ByteLength { get; }
        public string Key { get; private set; }
        public string Input { get; private set; }

        public DialogFormUpdatePackage(int instanceId, string key, string input)
        {
            InstanceId = instanceId;
            Key = key;
            Input = input;

            _keyBytes = Encoding.Unicode.GetBytes(key);
            _inputBytes = Encoding.Unicode.GetBytes(input);

            ByteLength = sizeof(int) + sizeof(int) + sizeof(int) + _keyBytes.Length + _inputBytes.Length;
        }

        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            byte[] instanceIdBytes = BitConverter.GetBytes(InstanceId);
            Array.Copy(instanceIdBytes, 0, package, pos, instanceIdBytes.Length);
            pos += sizeof(int);
            byte[] keyLengthBytes = BitConverter.GetBytes(_keyBytes.Length);
            byte[] inputLengthBytes = BitConverter.GetBytes(_inputBytes.Length);
            Array.Copy(keyLengthBytes, 0, package, pos, keyLengthBytes.Length);
            pos += keyLengthBytes.Length;
            Array.Copy(inputLengthBytes, 0, package, pos, inputLengthBytes.Length);
            pos += inputLengthBytes.Length;
            Array.Copy(_keyBytes, 0, package, pos, _keyBytes.Length);
            pos += _keyBytes.Length;
            Array.Copy(_inputBytes, 0, package, pos, _inputBytes.Length);
            pos += _inputBytes.Length;
            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            InstanceId = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            int keyLength = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            int inputLength = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            Key = Encoding.Unicode.GetString(data, pos, keyLength);
            pos += keyLength;
            Input = Encoding.Unicode.GetString(data, pos, inputLength);
            pos += inputLength;
        }
    }
}