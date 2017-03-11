using System.Threading;

namespace Kronos.Core.Storage.Cleaning
{
    internal class Scheduler : IScheduler
    {
        public const int DefaultPeriod = 5000;

        private readonly int _period;
        private Timer _timer;

        public Scheduler() : this(DefaultPeriod)
        {
        }

        internal Scheduler(int period)
        {
            _period = period;
        }

        public void Register(TimerCallback callback)
        {
            _timer = new Timer(callback, null, 0, _period);
        }
    }
}
