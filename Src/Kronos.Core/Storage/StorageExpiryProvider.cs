using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Kronos.Core.Storage
{
    public class StorageExpiryProvider : IExpiryProvider
    {
        public static readonly int Timer = 1000;

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void Start(Dictionary<Key, byte[]> nodes, CancellationToken token)
        {
            Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    long ticks = DateTime.UtcNow.Ticks;
                    ulong deleted = 0;
                    foreach (KeyValuePair<Key, byte[]> node in nodes)
                    {
                        if (node.Key.ExpiryDate.Ticks < ticks)
                        {
                            nodes.Remove(node.Key);

                            deleted++;
                        }
                    }
                    if (deleted > 0)
                    {
                        _logger.Info($"Deleted {deleted} elements from storage");
                    }

                    await Task.Delay(Timer, token);
                }

            }, TaskCreationOptions.LongRunning);
        }
    }
}

