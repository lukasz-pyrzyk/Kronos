using System.Collections.Concurrent;
using System.Threading;

namespace Kronos.Core.Storage
{
    internal interface IExpiryProvider
    {
        void Start(ConcurrentDictionary<NodeMetatada, byte[]> nodes, CancellationToken token);
    }
}