using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.GameManagement.SoundManagement;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Networking.UdpNetworkServer;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.ResourceManagement;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.TileEngine.TypeManagement;
using TileEngineSfmlCs.Utils.Serialization;

namespace TileEngineSfmlCs
{
    class Program
    {
        class TimeProvider : ITimeProvider
        {
            public event Action NextFrameEvent;
            public double DeltaTime { get; set; }
            public double TotalTime { get; set; }

            public void SendTimeSignal()
            {
                NextFrameEvent?.Invoke();
                Thread.Sleep(16); // 60 FPS
            }
        }

        class ConsoleLogger : ILogger
        {
            public void Log(string message)
            {
                DateTime now = DateTime.Now;
                string time = now.ToShortTimeString();
                Console.WriteLine($"[Server Log {time}] {message}");
            }

            public void LogError(string message)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                DateTime now = DateTime.Now;
                string time = now.ToShortTimeString();
                Console.WriteLine($"[Server Log {time}] {message}");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        static void Main(string[] args)
        {
            TimeProvider timeProvider = new TimeProvider();
            TimeManager.Instance = new TimeManager(timeProvider);
            LogManager.RuntimeLogger = new ConsoleLogger();

            GameManager.Instance = new GameManager();
            GameResources.Instance = new GameResources("C:\\Users\\Dmitry\\Documents\\GitHub\\TileEngineSfml\\TileEngineSfmlCs\\TileEngineSfmlCs\\Resources");
            MapContainerManager.Instance = new MapContainerManager();
            TypeManager.Instance = new TypeManager();
            SoundManager.Instance = new SoundManager();

            IMapContainer container = MapContainerManager.Instance.GetMapContainer("C:\\Users\\Dmitry\\Downloads\\_DELETE\\MapExample");
            Scene scene = Scene.CreateFromMap(container, "main.scene");
            
            //Scene scene = new Scene(50, 50);
            UdpNetworkServer server = new UdpNetworkServer(25565);
            GameManager.Instance.StartGame(scene, server);

            Console.WriteLine("TileEngine server started!");

            Stopwatch stopwatch = new Stopwatch();

            while (true)
            {
                stopwatch.Reset();
                stopwatch.Start();
                timeProvider.SendTimeSignal();
                stopwatch.Stop();
                timeProvider.DeltaTime = stopwatch.ElapsedMilliseconds / 1000f;
                timeProvider.TotalTime += timeProvider.DeltaTime;
            }
        }
    }
}
