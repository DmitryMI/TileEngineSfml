using System.Diagnostics;

namespace TileEngineSfmlCs.TileEngine.Logging
{
    public class DebugLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine("[Log] " + message);
        }

        public void LogError(string message)
        {
            Debug.WriteLine("[Log error] " + message);
        }
    }
}
