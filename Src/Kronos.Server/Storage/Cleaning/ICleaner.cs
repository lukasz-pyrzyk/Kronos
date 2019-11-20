using System.Collections.Generic;

namespace Kronos.Server.Storage.Cleaning
{
    interface ICleaner
    {
        void Clear(ConcurrentPriorityQueue<ExpiringKey> expiringKeys, Dictionary<Key, Element> nodes);
    }
}