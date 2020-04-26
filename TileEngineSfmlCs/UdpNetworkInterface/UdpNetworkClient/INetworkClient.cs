using System;

namespace UdpNetworkInterface.UdpNetworkClient
{
    public interface INetworkClient
    {
        event Action OnConnectionAccepted;
        event Action OnDisconnect;
        event Action<byte[]> OnDataReceived;

        ulong ConnectionCode { get; }

        void Connect(ulong code);
        void Disconnect();

        void Poll();
    }
}