namespace TileEngineSfmlCs.Logging
{
    public static class LogManager
    {
        public static ILogger RuntimeLogger { get; set; } = new DebugLogger();
        public static ILogger EditorLogger { get; set; } = new DebugLogger();
    }
}
