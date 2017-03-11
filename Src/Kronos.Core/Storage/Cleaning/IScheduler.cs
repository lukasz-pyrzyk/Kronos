using System.Threading;

namespace Kronos.Core.Storage.Cleaning
{
    internal interface IScheduler
    {
        void Register(TimerCallback callback);
    }
}
