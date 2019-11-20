using System.Threading;
using Kronos.Core.Configuration;

namespace Kronos.Server.Storage.Cleaning
{
    public interface IScheduler
    {
        void Register(TimerCallback callback, int period = DefaultSettings.CleanupTimeMs);
    }
}
