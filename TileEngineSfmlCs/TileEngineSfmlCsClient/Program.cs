using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML.Audio;
using TileEngineSfmlCs.GameManagement;
using TileEngineSfmlCs.GameManagement.ClientSide;
using TileEngineSfmlCs.GameManagement.ClientSide.TileObjects;
using TileEngineSfmlCs.GameManagement.DialogForms;
using TileEngineSfmlCs.GameManagement.DialogForms.Lobby;
using TileEngineSfmlCs.GameManagement.DialogForms.Main;
using TileEngineSfmlCs.GameManagement.SoundManagement;
using TileEngineSfmlCs.Logging;
using TileEngineSfmlCs.Networking.UdpNetworkClient;
using TileEngineSfmlCs.TileEngine.ResourceManagement;
using TileEngineSfmlCs.TileEngine.TimeManagement;
using TileEngineSfmlCs.Types;
using TileEngineSfmlCsClient.DialogFormWrappers;

namespace TileEngineSfmlCsClient
{
    class Program
    {
        private static MainForm _mainForm;

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

        private static void OnDialogFormSpawned(DialogFormSpirit spirit)
        {
            if (spirit is LobbyDialogFormSpirit lobbyDialogFormSpirit)
            {
                LobbyDialogWrapper wrapper = new LobbyDialogWrapper(lobbyDialogFormSpirit);
                wrapper.Show();
            }
            else if (spirit is MainDialogFormSpirit mainDialogFormSpirit)
            {
                _mainForm.DialogSpirit = mainDialogFormSpirit;
            }
        }

        private static void Initialization(string serverIp, int serverPort, string username, string resourcePath)
        {
#if DEBUG || !DEBUG
            resourcePath = "C:\\Users\\Dmitry\\Documents\\GitHub\\TileEngineSfml\\TileEngineSfmlCs\\TileEngineSfmlCs\\Resources";
            username = "Dmitry";
#endif
            GameResources.Instance = new GameResources(resourcePath);
            DialogFormManager.Instance = new DialogFormManager();
            DialogFormManager.Instance.OnDialogSpiritSpawned += OnDialogFormSpawned;
            ClientSoundManager.Instance = new ClientSoundManager();

            IPEndPoint serverEp = new IPEndPoint(IPAddress.Parse(serverIp), serverPort );
            UdpNetworkClient networkClient = new UdpNetworkClient(serverEp);
            ClientNetworkManager.Instance = new ClientNetworkManager(networkClient, username);
            ClientNetworkManager.Instance.OnConnectionAcceptedEvent += OnConnectionAccepted;
            ClientNetworkManager.Instance.OnConnectionTimeoutEvent += OnConnectionTimeout;
            ClientNetworkManager.Instance.Connect(5);
        }

        private static void OnConnectionAccepted(Vector2Int sceneSize)
        {
            TileSpiritManager.Instance = new TileSpiritManager(sceneSize.X, sceneSize.Y, _mainForm);
        }

        private static void OnConnectionTimeout()
        {
            LogManager.RuntimeLogger.LogError("Connection timeout!");
        }

        static void OnMainFormClose(object senser, EventArgs args)
        {
            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            //LogManager.RuntimeLogger = new ConsoleLogger();
            LoopTimeProvider timeProvider = new LoopTimeProvider();
            TimeManager.Instance = new TimeManager(timeProvider);
            double minimalDeltaTime = 0.02;

            _mainForm = new MainForm();
            _mainForm.Closed += OnMainFormClose;
            _mainForm.Show();

            LogManager.RuntimeLogger = _mainForm;

            Initialization("192.168.1.9", 25565, "Dmitry", "Resources");

            while (true)
            {
                Application.EnableVisualStyles();

                while (true)
                {
                    stopwatch.Restart();
                    {

                        Application.DoEvents();
                        timeProvider.SendTimeSignal();

                        GC.Collect();

                        double auxDelay = minimalDeltaTime - timeProvider.DeltaTime;
                        if (auxDelay > 0)
                        {
                            Thread.Sleep((int) (auxDelay * 1000));
                        }
                    }
                    stopwatch.Stop();
                    double deltaTime = stopwatch.ElapsedMilliseconds / 1000.0;
                    timeProvider.DeltaTime = deltaTime;
                    timeProvider.TotalTime += deltaTime;
                }
            }
        }
    }
}
