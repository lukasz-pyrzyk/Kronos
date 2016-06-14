using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Kronos.Core.Storage
{
    public class StorageExpiryProvider
    {
        private const int _timer = 1000;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentQueue<NodeMetatada> _nodesToRemove = new ConcurrentQueue<NodeMetatada>();

        public void StartWork(ConcurrentDictionary<NodeMetatada, byte[]> nodes, CancellationTokenSource cancel)
        {
            Task findJob = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (cancel.IsCancellationRequested) break;

                    Parallel.ForEach(nodes, node =>
                    {
                        if (node.Key.ExpiryDate < DateTime.Now)
                        {
                            _logger.Debug($"Moving to key {node} finalization");
                            _nodesToRemove.Enqueue(node.Key);
                        }
                    });

                    await Task.Delay(_timer);
                }
            }, TaskCreationOptions.LongRunning);

            Task removeJob = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (cancel.IsCancellationRequested) break;

                    while (!_nodesToRemove.IsEmpty)
                    {
                        if (cancel.IsCancellationRequested) break;

                        NodeMetatada metadata;
                        if (_nodesToRemove.TryDequeue(out metadata))
                        {
                            _logger.Debug($"Removing key {metadata} from storage");
                            byte[] data;
                            nodes.TryRemove(metadata, out data);
                        }
                    }

                    await Task.Delay(_timer);
                }
            }, TaskCreationOptions.LongRunning);

            Task.WaitAll(findJob, removeJob);
        }
    }
}

