using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.TileEngine.TimeManagement;

namespace TileEngineSfmlCsClient
{
    class Program
    {
        class LoopTimeProvider : ITimeProvider
        {
            public event Action NextFrameEvent;
            public double DeltaTime { get; set; }
            public double TotalTime { get; set; }

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
                Console.WriteLine($"[Log {time}] {message}");
            }

            public void LogError(string message)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                DateTime now = DateTime.Now;
                string time = now.ToShortTimeString();
                Console.WriteLine($"[Log {time}] {message}");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            LogManager.RuntimeLogger = new ConsoleLogger();
            LoopTimeProvider timeProvider = new LoopTimeProvider();
            TimeManager.Instance = new TimeManager(timeProvider);
            double minimalDeltaTime = 0.02;

            MainForm mainForm = new MainForm();
            mainForm.Show();
            while (true)
            {
                Application.EnableVisualStyles();

                while (true)
                {
                    stopwatch.Restart();
                    Application.DoEvents();
                    timeProvider.SendTimeSignal();
                    stopwatch.Stop();

                    double deltaTime = stopwatch.ElapsedMilliseconds / 1000.0;
                    timeProvider.DeltaTime = deltaTime;
                    timeProvider.TotalTime += deltaTime;

                    double auxDelay = minimalDeltaTime - deltaTime;
                    if (auxDelay > 0)
                    {
                        Thread.Sleep((int) (auxDelay * 1000));
                    }

                }
            }
        }
    }
}
