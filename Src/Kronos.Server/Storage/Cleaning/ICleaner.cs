using System.Collections.Generic;

namespace Kronos.Server.Storage.Cleaning
{
    interface ICleaner
    {
        void Clear(PriorityQueue<ExpiringKey> expiringKeys, Dictionary<Key, Element> nodes);
    }
}