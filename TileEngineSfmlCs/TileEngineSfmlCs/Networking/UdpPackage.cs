using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UdpNetworkInterface;

namespace TileEngineSfmlCs.Networking
{
    public struct UdpPackage
    {
        public UdpCommand Command { get; private set; }
        public Reliability Reliability { get; private set; }
        public ulong ConfirmationToken { get; private set; }
        public byte[] Payload { get; private set; }

        public int Length { get; private set; }

        public UdpPackage(UdpCommand command, Reliability reliability, ulong token, byte[] payload)
        {
            if (payload == null)
            {
                payload = new byte[0];
            }

            Command = command;
            Reliability = reliability;
            ConfirmationToken = token;
            Payload = payload;
            Length = 1 + 1 + payload.Length + sizeof(ulong);
        }

        public void FromByteArray(byte[] package, int start)
        {
            int pos = start;

            Command = (UdpCommand) package[pos];
            pos += 1;
            Reliability = (Reliability) package[pos];
            pos += 1;

            ConfirmationToken = BitConverter.ToUInt64(package, pos);
            pos += sizeof(ulong);

            Payload = new byte[package.Length - pos];
            Array.Copy(package, pos, Payload, 0, Payload.Length);
            pos += Payload.Length;

            Length = pos;
        }

        public void ToByteArray(byte[] package, int start)
        {
            int pos = start;
            byte[] commandBytes = new byte[1]{(byte)Command};
            byte[] reliabilityBytes = new byte[1] {(byte) Reliability};
            byte[] tokenBytes = BitConverter.GetBytes(ConfirmationToken);
            Array.Copy(commandBytes, 0, package, pos, commandBytes.Length);
            pos += commandBytes.Length;
            Array.Copy(reliabilityBytes, 0, package, pos, reliabilityBytes.Length);
            pos += reliabilityBytes.Length;

            Array.Copy(tokenBytes, 0, package, pos, tokenBytes.Length);
            pos += tokenBytes.Length;

            Array.Copy(Payload, 0, package, pos, Payload.Length);
            pos += Payload.Length;
        }
    }
}
