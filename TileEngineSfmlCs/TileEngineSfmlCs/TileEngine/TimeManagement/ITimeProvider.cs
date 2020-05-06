using System;

namespace TileEngineSfmlCs.TileEngine.TimeManagement
{
    public interface ITimeProvider
    {
        event Action NextFrameEvent;
        double DeltaTime { get; }
        double TotalTime { get; }
    }
}