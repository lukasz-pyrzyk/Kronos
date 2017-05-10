using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using Kronos.Core.Storage.Cleaning;
using NLog;

namespace Kronos.Core.Storage
{
    public class InMemoryStorage : IStorage
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Key, Element> _storage = new Dictionary<Key, Element>();
        private readonly PriorityQueue<ExpiringKey> _expiringKeys = new PriorityQueue<ExpiringKey>();

        private int cleanupRequested;
        private readonly ICleaner _cleaner;
        private readonly IScheduler _scheduler;

        public InMemoryStorage() : this(new Cleaner(), new Scheduler())
        {
        }

        internal InMemoryStorage(ICleaner cleaner, IScheduler scheduler)
        {
            _cleaner = cleaner;
            _scheduler = scheduler;
            _scheduler.Register(OnTimer);
        }

        public int Count => _storage.Count;
        public int ExpiringCount => _expiringKeys.Count;
        internal bool CleanupRequested => cleanupRequested == 1;

        public bool Add(string name, DateTime? expiryDate, ByteString obj)
        {
            ClearStorageIfRequested();

            var key = new Key(name);

            if (_storage.ContainsKey(key))
                return false;

            var element = new Element(obj, expiryDate);
            _storage[key] = element;

            if (expiryDate.HasValue)
            {
                _expiringKeys.Add(new ExpiringKey(key, expiryDate.Value));
            }

            return true;
        }

        public bool TryGet(string name, out ByteString obj)
        {
            ClearStorageIfRequested();

            var key = new Key(name);
            Element element;
            bool found = _storage.TryGetValue(key, out element);
            if (found && !element.IsExpired())
            {
                obj = element.Data;
                return true;
            }

            obj = null;
            return false;
        }

        public bool TryRemove(string name)
        {
            ClearStorageIfRequested();

            var key = new Key(name);
            Element element;
            bool found = _storage.TryGetValue(key, out element);
            if (found)
            {
                _storage.Remove(key);
                if (element.IsExpiring)
                {
                    _expiringKeys.Remove(new ExpiringKey(key, default(DateTime)));
                }
            }

            return found;
        }

        public bool Contains(string name)
        {
            ClearStorageIfRequested();

            var key = new Key(name);
            Element element;
            bool found = _storage.TryGetValue(key, out element);

            return found && !element.IsExpired();
        }

        public int Clear()
        {
            _logger.Info("Clearing storage");

            int count = Count;
            _storage.Clear();
            _expiringKeys.Clear();

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
                _logger.Debug("Clearing storage");
                _cleaner.Clear(_expiringKeys, _storage);
                Interlocked.Exchange(ref cleanupRequested, 0);
            }
        }

        private void OnTimer(object obj)
        {
            // try to request for cleanup
            if (Interlocked.CompareExchange(ref cleanupRequested, 1, 0) == 0)
            {
                _logger.Debug("Storage cleanup scheduled");
            }
        }
    }
}
