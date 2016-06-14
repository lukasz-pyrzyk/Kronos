using System.Collections.Concurrent;
using System.Threading;
using NLog;

namespace Kronos.Core.Storage
{
    public class InMemoryStorage : IStorage
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<NodeMetatada, byte[]> _storage =
            new ConcurrentDictionary<NodeMetatada, byte[]>(new NodeComparer());

        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        public int Count => _storage.Count;

        public InMemoryStorage()
        {
            var provider = new StorageExpiryProvider();
            provider.StartWork(_storage, _cancel);
        }

        public void AddOrUpdate(string key, byte[] obj)
        {
            _logger.Debug($"Inserting key {key} to MemoryStorage");
            var metaData = new NodeMetatada(key);
            _storage.AddOrUpdate(metaData, obj, (s, bytes) => obj);
        }

        public byte[] TryGet(string key)
        {
            _logger.Debug($"Getting package for key {key}");
            var metaData = new NodeMetatada(key);
            byte[] obj;
            if (_storage.TryGetValue(metaData, out obj))
            {
                _logger.Debug($"Returning object of key {key}");
                return obj;
            }

            _logger.Debug($"Key {key} not found. Returning null");
            return null;
        }

        public bool TryRemove(string key)
        {
            byte[] obj;
            var metaData = new NodeMetatada(key);
            return _storage.TryRemove(metaData, out obj);
        }

        public void Dispose()
        {
            _logger.Info("Disposing storage");

            _cancel.Cancel();
            _storage.Clear();
        }
    }
}
