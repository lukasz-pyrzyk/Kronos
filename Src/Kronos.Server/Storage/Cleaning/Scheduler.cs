using System;
using System.Threading;
using Kronos.Core.Configuration;

namespace Kronos.Server.Storage.Cleaning
{
    class Scheduler : IScheduler, IDisposable
    {
        private Timer _timer;

        public void Register(TimerCallback callback, int period = DefaultSettings.CleanupTimeMs)
        {
            _timer = new Timer(callback, null, 0, period);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
