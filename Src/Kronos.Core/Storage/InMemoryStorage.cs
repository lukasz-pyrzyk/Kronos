using System;
using System.Collections.Generic;
using System.Threading;
using NLog;

namespace Kronos.Core.Storage
{
    public class InMemoryStorage : IStorage
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IExpiryProvider _expiryProvider;

        private readonly Dictionary<NodeMetatada, byte[]> _storage =
            new Dictionary<NodeMetatada, byte[]>(new NodeComparer());

        private readonly NodesPool _pool = new NodesPool();

        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public int Count => _storage.Count;

        public InMemoryStorage(IExpiryProvider expiryProvider)
        {
            _expiryProvider = expiryProvider;
            _expiryProvider.Start(_storage, _cancelToken.Token);
        }

        public void AddOrUpdate(string key, DateTime expiryDate, byte[] obj)
        {
            var metaData = _pool.Rent();
            metaData.Reuse(key, expiryDate);

            _storage[metaData] = obj;
        }

        public bool TryGet(string key, out byte[] obj)
        {
            var metaData = _pool.Rent();
            metaData.Reuse(key);

            bool found = _storage.TryGetValue(metaData, out obj);
            _pool.Return(metaData);
            return found;
        }

        public bool TryRemove(string key)
        {
            var metaData = _pool.Rent();
            metaData.Reuse(key);

            bool removed = _storage.Remove(metaData);
            _pool.Return(metaData);
            return removed;
        }

        public bool Contains(string key)
        {
            var metaData = _pool.Rent();
            metaData.Reuse(key);

            bool contains = _storage.ContainsKey(metaData);
            _pool.Return(metaData);
            return contains;
        }

        public void Clear()
        {
            _logger.Info("Clearing storage");
            _storage.Clear();
        }

        public void Dispose()
        {
            _logger.Info("Disposing storage");

            _cancelToken.Cancel();
            Clear();
        }
    }
}
