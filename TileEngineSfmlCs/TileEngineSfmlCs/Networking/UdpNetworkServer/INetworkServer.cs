using System;
using TileEngineSfmlCs.GameManagement;

namespace TileEngineSfmlCs.Networking.UdpNetworkServer
{
    public interface INetworkServer : IDisposable
    {
        event Action<int, string> OnNewConnection;
        event Action<int> OnDisconnect;
        event Action<int> OnReconnect;
        event Action<int, byte[]> OnDataReceived;

        Func<byte[]> NewConnectionResponse { get; set; }

        void SendData(int connectionId, byte[] data, Reliability reliability = Reliability.Unreliable);

        void StartServer();
        void StopServer();

        void Poll();

        bool NewConnectionsEnabled { get; set; }

        bool IsListening { get; }

        int[] ConnectionIds { get; }
    }
}