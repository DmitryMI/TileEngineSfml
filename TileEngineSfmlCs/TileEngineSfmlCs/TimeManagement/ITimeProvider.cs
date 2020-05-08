using System;

namespace TileEngineSfmlCs.TimeManagement
{
    public interface ITimeProvider
    {
        event Action NextFrameEvent;
        double DeltaTime { get; }
        double TotalTime { get; }
    }
}