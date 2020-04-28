using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngineSfmlCs.TileEngine.Logging;
using TileEngineSfmlCs.TileEngine.TimeManagement;

namespace TileEngineSfmlCs.Utils
{
    public class DelayedAction
    {
        private static List<DelayedAction> _activeDelayedActions = new List<DelayedAction>();

        private ITimeProvider _timeProvider;
        private Queue<Action<DelayedAction, object>> _delayedActions = new Queue<Action<DelayedAction, object>>();
        private Queue<object> _arguments = new Queue<object>();
        private Queue<float> _times = new Queue<float>();
        private bool _selfDestroy = true;

        public DelayedAction(ITimeProvider timeProvider, bool selfDestroy = true)
        {
            _timeProvider = timeProvider;
            _timeProvider.NextFrameEvent += NextFrame;
            _selfDestroy = selfDestroy;

            _activeDelayedActions.Add(this);

            LogManager.RuntimeLogger.Log("[DelayedAction] Created new delayed action");
        }

        public void Delay(Action<DelayedAction, object> action, object argument, float delay)
        {
            _delayedActions.Enqueue(action);
            _arguments.Enqueue(argument);
            _times.Enqueue(_timeProvider.TotalTime + delay);
        }

        private void NextFrame()
        {
            if (_delayedActions.Count == 0 && _selfDestroy)
            {
                Destroy();
            }
            LogManager.RuntimeLogger.Log($"Actions count:{_delayedActions.Count}. Time: {_timeProvider.TotalTime:0.00}. Nearest action: {(_times.Count > 0 ? _times.Peek() : 0):0.00}");
            for (int i = 0; i < _delayedActions.Count; i++)
            {
                float time = _times.Peek();
                if (_timeProvider.TotalTime >= time)
                {
                    Debug.WriteLine("DelayedAction invocation!");
                    _times.Dequeue();
                    Action<DelayedAction, object> action = _delayedActions.Dequeue();
                    object argument = _arguments.Dequeue();
                    action?.Invoke(this, argument);
                }
                else
                {
                    break;
                }
            }
        }

        public void Destroy()
        {
            LogManager.RuntimeLogger.Log("[DelayedAction] Destroyed");
            _timeProvider.NextFrameEvent -= NextFrame;
            _activeDelayedActions.Remove(this);
        }
    }
}
