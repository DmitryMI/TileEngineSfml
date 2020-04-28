using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.Networking.UdpNetworkServer;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.TileEngine.TimeManagement;

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
            
            Scene scene = new Scene(50, 50);
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
