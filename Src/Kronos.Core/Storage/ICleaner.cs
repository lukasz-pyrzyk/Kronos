using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;

namespace Kronos.Core.Storage
{
    public interface ICleaner
    {
        void Start(Dictionary<Key, ByteString> nodes, CancellationToken token);
    }
}