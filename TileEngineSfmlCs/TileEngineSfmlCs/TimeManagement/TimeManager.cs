using System;

namespace TileEngineSfmlCs.TimeManagement
{
    /// <summary>
    /// Singleton wrapper for abstract time providers
    /// </summary>
    public class TimeManager : ITimeProvider
    {
        #region Singleton

        private static TimeManager _instance;

        public static TimeManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        #endregion

        private ITimeProvider _timeProvider;

        public TimeManager(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
            _timeProvider.NextFrameEvent += OnNextFrame;
        }

        private void OnNextFrame()
        {
            NextFrameEvent?.Invoke();
        }

        public event Action NextFrameEvent;

        public double DeltaTime
        {
            get => _timeProvider.DeltaTime;
        }

        public double TotalTime
        {
            get => _timeProvider.TotalTime;
        }
    }
}
