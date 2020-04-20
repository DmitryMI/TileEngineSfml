using System;

namespace TileEngineSfmlCs.TileEngine.TimeManagement
{
    public interface ITimeProvider
    {
        event Action NextFrameEvent;
        float DeltaTime { get; }
        float TotalTime { get; }
    }
}