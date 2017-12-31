using System;
using System.Collections.Generic;
using NLog;

namespace Kronos.Core.Storage.Cleaning
{
    internal class Cleaner : ICleaner
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

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
                Logger.Info($"Deleted {deleted} elements from storage");
            }
        }
    }
}