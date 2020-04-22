using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.Logging
{
    public static class LogManager
    {
        public static ILogger RuntimeLogger { get; set; } = new DebugLogger();
        public static ILogger EditorLogger { get; set; } = new DebugLogger();
    }
}
