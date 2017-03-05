using System.Collections.Generic;
using Google.Protobuf;

namespace Kronos.Core.Storage
{
    public interface ICleaner
    {
        void Clear(PriorityQueue<Key> expiringKeys, Dictionary<Key, ByteString> nodes);
    }
}