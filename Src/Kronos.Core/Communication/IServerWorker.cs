using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Storage;

namespace Kronos.Core.Communication
{
    public interface IServerWorker
    {
        IStorage Storage { get; }
        Task StartListeningAsync(CancellationToken token);
    }
}
