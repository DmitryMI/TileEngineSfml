using System.Net;

namespace TileEngineSfmlCs.Networking
{
    public class Retransmission
    {
        public IPEndPoint EndPoint { get; set; }
        public byte[] DataBuffer { get; set; }
        public ulong ConfirmationToken { get; set; }
        public int RetriesRemaining { get; set; }

        public Retransmission(IPEndPoint endPoint, byte[] data, ulong token, int retries)
        {
            EndPoint = endPoint;
            DataBuffer = data;
            ConfirmationToken = token;
            RetriesRemaining = retries;
        }
    }
}