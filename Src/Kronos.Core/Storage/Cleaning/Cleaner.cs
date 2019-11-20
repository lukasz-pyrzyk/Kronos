using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Kronos.Core.Storage.Cleaning
{
    internal class Cleaner : ICleaner
    {
        private readonly ILogger<Cleaner> _logger;

        public Cleaner(ILogger<Cleaner> logger)
        {
            _logger = logger;
        }

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
                _logger.LogDebug("Deleted {deleted} elements from storage", deleted);
            }
        }
    }
}