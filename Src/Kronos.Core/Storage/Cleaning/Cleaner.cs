using System;
using NLog;

namespace Kronos.Core.Storage.Cleaning
{
    internal class Cleaner : ICleaner
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public void Clear(PriorityQueue<ExpiringKey> expiringKeys, IStorage storage)
        {
            var date = DateTimeOffset.UtcNow;
            uint deleted = 0;

            while (expiringKeys.Count > 0 && expiringKeys.Peek().IsExpired(date))
            {
                ExpiringKey expiringKey = expiringKeys.Poll();
                storage.TryRemove(expiringKey.Key.Name);
                deleted++;
            }

            if (deleted > 0)
            {
                Logger.Info($"Deleted {deleted} elements from storage");
            }
        }
    }
}