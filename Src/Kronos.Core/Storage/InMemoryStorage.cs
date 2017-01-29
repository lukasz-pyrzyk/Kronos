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

        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public int Count => _storage.Count;

        public InMemoryStorage(IExpiryProvider expiryProvider)
        {
            _expiryProvider = expiryProvider;
            _expiryProvider.Start(_storage, _cancelToken.Token);
        }

        public void AddOrUpdate(string key, DateTime expiryDate, byte[] obj)
        {
            var metaData = new NodeMetatada(key, expiryDate);

            _storage[metaData] = obj;
        }

        public bool TryGet(string key, out byte[] obj)
        {
            var metaData = new NodeMetatada(key);

            return _storage.TryGetValue(metaData, out obj);
        }

        public bool TryRemove(string key)
        {
            var metaData = new NodeMetatada(key);
            return _storage.Remove(metaData);
        }

        public bool Contains(string key)
        {
            return _storage.ContainsKey(new NodeMetatada(key));
        }

        public void Dispose()
        {
            _logger.Info("Disposing storage");

            _cancelToken.Cancel();
            _storage.Clear();
        }
    }
}
