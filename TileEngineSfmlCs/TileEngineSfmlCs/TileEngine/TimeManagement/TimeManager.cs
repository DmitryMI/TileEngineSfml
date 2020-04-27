using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.TimeManagement
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

        public float DeltaTime
        {
            get => _timeProvider.DeltaTime;
        }

        public float TotalTime
        {
            get => _timeProvider.TotalTime;
        }
    }
}
