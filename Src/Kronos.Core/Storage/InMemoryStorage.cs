using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using NLog;

namespace Kronos.Core.Storage
{
    public class InMemoryStorage : IStorage
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Key, ByteString> _storage =
            new Dictionary<Key, ByteString>(new KeyComperer());

        private readonly PriorityQueue<Key> _expiringKeys = new PriorityQueue<Key>();

        private int cleanupRequested;
        private readonly Timer _timer;
        private readonly ICleaner _cleaner;

        public InMemoryStorage() : this(new Cleaner())
        {
        }

        public InMemoryStorage(ICleaner cleaner)
        {
            _timer = new Timer(OnCleanupTimer, null, 0, 5000);
            _cleaner = cleaner;
        }
        
        public int Count => _storage.Count;
        public int ExpiringCount => _expiringKeys.Count;

        public bool Add(string key, DateTime? expiryDate, ByteString obj)
        {
            ClearStorageIfRequested();

            var metaData = new Key(key, expiryDate);

            if (_storage.ContainsKey(metaData))
                return false;

            _storage[metaData] = obj;
            _expiringKeys.Add(metaData);

            return true;
        }

        public void AddOrUpdate(string key, DateTime? expiryDate, ByteString obj)
        {
            ClearStorageIfRequested();

            var metaData = new Key(key, expiryDate);

            _storage[metaData] = obj;
            _expiringKeys.Add(metaData);
        }

        public bool TryGet(string key, out ByteString obj)
        {
            ClearStorageIfRequested();

            var metaData = new Key(key);
            return _storage.TryGetValue(metaData, out obj);
        }

        public bool TryRemove(string key)
        {
            ClearStorageIfRequested();

            var metaData = new Key(key);
            return _storage.Remove(metaData);
        }

        public bool Contains(string key)
        {
            ClearStorageIfRequested();

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

            Clear();
        }

        private void ClearStorageIfRequested()
        {
            // check if cleanup was requested, do not change value
            if (Interlocked.CompareExchange(ref cleanupRequested, 1, 1) == 1)
            {
                _logger.Info("Clearing storage");
                _cleaner.Clear(_expiringKeys, _storage);
                Interlocked.Exchange(ref cleanupRequested, 0);
            }
        }

        private void OnCleanupTimer(object obj)
        {
            // try to request for cleanup
            if (Interlocked.CompareExchange(ref cleanupRequested, 1, 0) == 0)
            {
                _logger.Info("Storage cleanup scheduled");
            }
        }
    }
}
