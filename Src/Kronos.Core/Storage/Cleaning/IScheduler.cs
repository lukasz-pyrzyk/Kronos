using System.Threading;
using Kronos.Core.Configuration;

namespace Kronos.Core.Storage.Cleaning
{
    public interface IScheduler
    {
        void Register(TimerCallback callback, int period = DefaultSettings.CleanupTimeMs);
    }
}
