using System.Net.Sockets;
using Kronos.Core.Storage;

namespace Kronos.Core.Communication
{
    public interface IServerWorker
    {
        IStorage Storage { get; }
        void StartListening(ISocket server);
    }
}
