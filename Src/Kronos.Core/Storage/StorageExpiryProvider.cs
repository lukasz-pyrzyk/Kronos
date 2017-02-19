using System;
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
                    DateTime currentDate = DateTime.UtcNow;
                    ulong deleted = 0;
                    foreach (Key key in nodes.Keys)
                    {
                        if (key.IsExpired(currentDate))
                        {
                            nodes.Remove(key);

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

