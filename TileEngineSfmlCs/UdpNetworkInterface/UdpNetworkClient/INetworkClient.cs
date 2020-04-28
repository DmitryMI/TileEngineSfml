using System;

namespace UdpNetworkInterface.UdpNetworkClient
{
    public interface INetworkClient
    {
        event Action OnConnectionAccepted;
        event Action OnDisconnect;
        event Action<byte[]> OnDataReceived;

        ulong ConnectionCode { get; }

        void Connect(string username, ulong token);
        void Disconnect();
        void Send(byte[] data);

        void Poll();
    }
}