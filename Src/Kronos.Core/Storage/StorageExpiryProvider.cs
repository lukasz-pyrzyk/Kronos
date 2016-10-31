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

        public void Start(ConcurrentDictionary<NodeMetatada, byte[]> nodes, CancellationToken token)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) break;

                    long ticks = DateTime.UtcNow.Ticks;
                    ulong deleted = 0;
                    foreach (KeyValuePair<NodeMetatada, byte[]> node in nodes)
                    {
                        if (node.Key.ExpiryDate.Ticks < ticks)
                        {
                            byte[] temp;
                            nodes.TryRemove(node.Key, out temp);

                            deleted++;
                        }
                    }

                    if(deleted > 0) _logger.Info($"Deleted {deleted} elements from storage");

                    await Task.Delay(Timer, token);
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}

