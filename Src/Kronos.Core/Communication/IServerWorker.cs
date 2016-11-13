using System;
using System.Threading;
using System.Threading.Tasks;
using Kronos.Core.Storage;

namespace Kronos.Core.Communication
{
    public interface IServerWorker : IDisposable
    {
        IStorage Storage { get; }

        void Start();
    }
}
