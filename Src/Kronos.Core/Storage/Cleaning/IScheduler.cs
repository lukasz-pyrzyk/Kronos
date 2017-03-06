using System.Threading;

namespace Kronos.Core.Storage.Cleaning
{
    public interface IScheduler
    {
        void Register(TimerCallback callback);
    }
}
