using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
