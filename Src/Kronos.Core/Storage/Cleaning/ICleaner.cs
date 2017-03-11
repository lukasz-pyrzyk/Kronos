using System.Collections.Generic;

namespace Kronos.Core.Storage.Cleaning
{
    internal interface ICleaner
    {
        void Clear(PriorityQueue<ExpiringKey> expiringKeys, Dictionary<Key, Element> nodes);
    }
}