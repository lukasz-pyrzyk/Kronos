using System;
using System.Collections.Generic;
using NLog;

namespace Kronos.Core.Storage.Cleaning
{
    public class Cleaner : ICleaner
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void Clear(PriorityQueue<ExpiringKey> expiringKeys, Dictionary<Key, Element> nodes)
        {
            DateTime date = DateTime.UtcNow;
            uint deleted = 0;

            while (expiringKeys.Count > 0 && expiringKeys.Peek().IsExpired(date))
            {
                ExpiringKey expiringKey = expiringKeys.Poll();
                nodes.Remove(expiringKey.Key);
                deleted++;
            }

            if (deleted > 0)
            {
                _logger.Info($"Deleted {deleted} elements from storage");
            }
        }
    }
}