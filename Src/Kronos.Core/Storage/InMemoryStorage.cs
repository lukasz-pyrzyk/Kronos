using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using NLog;

namespace Kronos.Core.Storage
{
    public class InMemoryStorage : IStorage
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICleaner _cleaner;

        private readonly Dictionary<Key, ByteString> _storage =
            new Dictionary<Key, ByteString>(new KeyComperer());

        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public int Count => _storage.Count;

        public InMemoryStorage(ICleaner cleaner)
        {
            _cleaner = cleaner;
            _cleaner.Start(_storage, _cancelToken.Token);
        }

        public bool Add(string key, DateTime? expiryDate, ByteString obj)
        {
            var metaData = new Key(key, expiryDate);
            if (_storage.ContainsKey(metaData))
                return false;

            _storage[metaData] = obj;
            return true;
        }

        public void AddOrUpdate(string key, DateTime? expiryDate, ByteString obj)
        {
            var metaData = new Key(key, expiryDate);

            _storage[metaData] = obj;
        }

        public bool TryGet(string key, out ByteString obj)
        {
            var metaData = new Key(key);
            return _storage.TryGetValue(metaData, out obj);
        }

        public bool TryRemove(string key)
        {
            var metaData = new Key(key);
            return _storage.Remove(metaData);
        }

        public bool Contains(string key)
        {
            var metaData = new Key(key);
            return _storage.ContainsKey(metaData);
        }

        public int Clear()
        {
            _logger.Info("Clearing storage");

            int count = Count;
            _storage.Clear();

            return count;
        }

        public void Dispose()
        {
            _logger.Info("Disposing storage");

            _cancelToken.Cancel();
            Clear();
        }
    }
}
