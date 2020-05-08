using System;
using System.Timers;

namespace TileEngineSfmlCs.TimeManagement
{
    public class TimerTimeProvider : ITimeProvider
    {
        private readonly Timer _timer;
        public event Action NextFrameEvent;
        public double DeltaTime { get; private set; }
        public double TotalTime { get; private set; }

        private long _prevOsTicks;


        public float IntervalMs
        {
            get => (float)_timer.Interval;
            set => _timer.Interval = value;
        }

        public void StartTimer()
        {
            _prevOsTicks = DateTime.Now.Ticks;
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        public TimerTimeProvider(float intervalMs)
        {
            _timer = new Timer(intervalMs);
            _timer.Elapsed += Elapsed;
        }

        private void Elapsed(object sender, EventArgs args)
        {
            long osTime = DateTime.Now.Ticks;
            float deltaTicks = osTime - _prevOsTicks;
            DeltaTime = deltaTicks / TimeSpan.TicksPerMillisecond / 1000;
            TotalTime += DeltaTime;
            NextFrameEvent?.Invoke();
        }

        
    }
}
