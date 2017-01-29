using System.Collections.Generic;
using System.Threading;

namespace Kronos.Core.Storage
{
    public interface IExpiryProvider
    {
        void Start(Dictionary<NodeMetatada, byte[]> nodes, CancellationToken token);
    }
}