using System;
using System.Collections.Generic;
using Google.Protobuf;
using NLog;

namespace Kronos.Core.Storage
{
    public class Cleaner : ICleaner
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void Clear(PriorityQueue<Key> expiringKeys, Dictionary<Key, ByteString> nodes)
        {
            DateTime date = DateTime.UtcNow;
            ulong deleted = 0;

            while (expiringKeys.Count > 0 && expiringKeys.Peek().IsExpired(date))
            {
                Key key = expiringKeys.Poll();
                nodes.Remove(key);
                deleted++;
            }

            if (deleted > 0)
            {
                _logger.Info($"Deleted {deleted} elements from storage");
            }
        }
    }
}

