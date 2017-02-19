using System.Collections.Generic;
using System.Threading;

namespace Kronos.Core.Storage
{
    public interface ICleaner
    {
        void Start(Dictionary<Key, byte[]> nodes, CancellationToken token);
    }
}