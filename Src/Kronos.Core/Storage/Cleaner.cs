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
            DateTime currentDate = DateTime.UtcNow;
            ulong deleted = 0;
            
            // TODO - implement

            if (deleted > 0)
            {
                _logger.Info($"Deleted {deleted} elements from storage");
            }
        }
    }
}

