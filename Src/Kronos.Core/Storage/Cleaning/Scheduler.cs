using System.Threading;
using Kronos.Core.Configuration;

namespace Kronos.Core.Storage.Cleaning
{
    public class Scheduler : IScheduler
    {
        private Timer _timer;

        public void Register(TimerCallback callback, int period = DefaultSettings.CleanupTimeMs)
        {
            _timer = new Timer(callback, null, 0, period);
        }
    }
}
