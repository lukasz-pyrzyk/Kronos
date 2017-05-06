using System.Threading;
using Kronos.Core.Configuration;

namespace Kronos.Core.Storage.Cleaning
{
    internal interface IScheduler
    {
        void Register(TimerCallback callback, int period = Settings.CleanupTimeMs);
    }
}
