using System;
using System.Diagnostics;
using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using UdpNetworkInterface.UdpNetworkServer;

namespace TileEngineSfmlCs
{
    class Program
    {
        class TimeProvider : ITimeProvider
        {
            public event Action NextFrameEvent;
            public float DeltaTime { get; set; }
            public float TotalTime { get; set; }

            public void SendTimeSignal()
            {
                NextFrameEvent?.Invoke();
            }
        }

        static void Main(string[] args)
        {
            TimeProvider timeProvider = new TimeProvider();
            GameManager gameManager = new GameManager();
            
            Scene scene = new Scene(50, 50);
            UdpNetworkServer server = new UdpNetworkServer(25565);
            gameManager.StartGame(scene, timeProvider, server);

            Console.WriteLine("TileEngine server started!");
            while (true)
            {
                timeProvider.SendTimeSignal();
            }
        }
    }
}
