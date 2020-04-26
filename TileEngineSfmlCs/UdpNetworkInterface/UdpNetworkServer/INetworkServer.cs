using System;

namespace UdpNetworkInterface.UdpNetworkServer
{
    public interface INetworkServer : IDisposable
    {
        event Action<int> OnNewConnection;
        event Action<int> OnDisconnect;
        event Action<int> OnReconnect;
        event Action<int, byte[]> OnDataReceived;
        void SendData(int connectionId, byte[] data);

        void StartServer();
        void StopServer();

        void Poll();

        bool NewConnectionsEnabled { get; set; }

        bool IsListening { get; }
    }
}