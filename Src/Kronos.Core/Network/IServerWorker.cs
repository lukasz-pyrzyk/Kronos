using System;
using Kronos.Core.Storage;

namespace Kronos.Core.Network
{
    public interface IServerWorker : IDisposable
    {
        IStorage Storage { get; }

        void Start();
    }
}
