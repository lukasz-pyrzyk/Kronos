using System.Collections.Generic;

namespace Kronos.Core.Storage.Cleaning
{
    public interface ICleaner
    {
        void Clear(PriorityQueue<ExpiringKey> expiringKeys, Dictionary<Key, Element> nodes);
    }
}