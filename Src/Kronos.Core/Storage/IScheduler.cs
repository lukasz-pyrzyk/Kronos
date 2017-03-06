using System.Threading;

namespace Kronos.Core.Storage
{
    public interface IScheduler
    {
        void Register(TimerCallback callback);
    }
}
