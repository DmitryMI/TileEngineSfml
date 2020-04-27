﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.GameManagement.ServerSide;
using TileEngineSfmlCs.TileEngine;
using TileEngineSfmlCs.TileEngine.Logging;
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

            float[] deltaTimeStats = new float[10000];
            int dtStatPos = 0;

            while (true)
            {
                stopwatch.Reset();
                stopwatch.Start();
                timeProvider.SendTimeSignal();
                stopwatch.Stop();
                timeProvider.DeltaTime = stopwatch.ElapsedMilliseconds / 1000f;
                timeProvider.TotalTime += timeProvider.DeltaTime;
                deltaTimeStats[dtStatPos] = timeProvider.DeltaTime;

                dtStatPos++;
                if (dtStatPos >= deltaTimeStats.Length)
                {
                    dtStatPos = 0;
                    float sumFps = 0;
                    for (int i = 0; i < deltaTimeStats.Length; i++)
                    {
                        sumFps += deltaTimeStats[i];
                    }

                    sumFps /= deltaTimeStats.Length;
                    //LogManager.RuntimeLogger.Log($"Average DT: {sumFps}");
                }

            }
        }
    }
}
