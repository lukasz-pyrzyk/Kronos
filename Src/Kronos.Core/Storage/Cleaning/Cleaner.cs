using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kronos.Core.Storage.Cleaning
{
    internal class Cleaner : ICleaner
    {
        public void Clear(PriorityQueue<ExpiringKey> expiringKeys, Dictionary<Key, Element> nodes)
        {
            DateTimeOffset date = DateTimeOffset.UtcNow;
            uint deleted = 0;

            while (expiringKeys.Count > 0 && expiringKeys.Peek().IsExpired(date))
            {
                ExpiringKey expiringKey = expiringKeys.Poll();
                nodes.Remove(expiringKey.Key);
                deleted++;
            }

            if (deleted > 0)
            {
                Trace.TraceInformation($"Deleted {deleted} elements from storage");
            }
        }
    }
}