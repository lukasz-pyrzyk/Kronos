using System;
using Kronos.Core.Storage;

namespace Kronos.Core.Communication
{
    public interface IServerWorker : IDisposable
    {
        IStorage Storage { get; }
        void StartListening();
    }
}
