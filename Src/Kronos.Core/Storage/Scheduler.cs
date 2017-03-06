using System.Threading;

namespace Kronos.Core.Storage
{
    public class Scheduler : IScheduler
    {
        public const int DefaultPeriod = 5000;

        private Timer _timer;

        public void Register(TimerCallback callback)
        {
            _timer = new Timer(callback, null, 0, DefaultPeriod);
        }
    }
}
