using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Kronos.Core.Pooling;
using Kronos.Core.Storage.Cleaning;
using NLog;

namespace Kronos.Core.Storage
{
    public class InMemoryStorage : IStorage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Key, Element> _storage = new Dictionary<Key, Element>();
        private readonly PriorityQueue<ExpiringKey> _expiringKeys = new PriorityQueue<ExpiringKey>();

        private int _cleanupRequested;
        private readonly ICleaner _cleaner;
        private readonly ServerMemoryPool _pool;

        public InMemoryStorage(ServerMemoryPool pool) : this(new Cleaner(), new Scheduler(), pool)
        {
        }

        internal InMemoryStorage(ICleaner cleaner, IScheduler scheduler, ServerMemoryPool pool)
        {
            _cleaner = cleaner;
            _pool = pool;
            scheduler.Register(OnTimer);
        }

        public int Count => _storage.Count;
        public int ExpiringCount => _expiringKeys.Count;
        internal bool CleanupRequested => _cleanupRequested == 1;

        public bool Add(string name, DateTimeOffset? expiryDate, ReadOnlyMemory<byte> memory)
        {
            ClearStorageIfRequested();

            var key = new Key(name);

            bool found = _storage.TryGetValue(key, out Element element);
            if (found && !element.IsExpired())
            {
                return false;
            }

            var memoryToStore = _pool.Rent(memory.Length);
            memory.Span.CopyTo(memoryToStore.Memory.Span);
            element = new Element(memoryToStore, expiryDate);
            _storage[key] = element;

            if (expiryDate.HasValue)
            {
                _expiringKeys.Add(new ExpiringKey(key, expiryDate.Value));
            }

            return true;
        }

        public bool TryGet(string name, out ReadOnlyMemory<byte> data)
        {
            ClearStorageIfRequested();

            var key = new Key(name);
            bool found = _storage.TryGetValue(key, out Element element);
            if (found && !element.IsExpired())
            {
                data = element.MemoryOwner.Memory;
                return true;
            }

            data = null;
            return false;
        }

        public bool TryRemove(string name)
        {
            ClearStorageIfRequested();

            var key = new Key(name);
            bool found = _storage.TryGetValue(key, out Element element);
            if (found)
            {
                element.MemoryOwner.Dispose();
                _storage.Remove(key);
            }

            return found;
        }

        public bool Contains(string name)
        {
            ClearStorageIfRequested();

            var key = new Key(name);
            bool found = _storage.TryGetValue(key, out Element element);

            return found && !element.IsExpired();
        }

        public int Clear()
        {
            Logger.Info("Clearing storage");

            int count = Count;
            _storage.Clear();
            _expiringKeys.Clear();

            return count;
        }

        public void Dispose()
        {
            Logger.Info("Disposing storage");

            Clear();
        }

        private void ClearStorageIfRequested()
        {
            // check if cleanup was requested, do not change value
            if (Interlocked.CompareExchange(ref _cleanupRequested, 1, 1) == 1)
            {
                Logger.Debug("Clearing storage");
                _cleaner.Clear(_expiringKeys, this);
                Interlocked.Exchange(ref _cleanupRequested, 0);
            }
        }

        private void OnTimer(object obj)
        {
            // try to request for cleanup
            if (Interlocked.CompareExchange(ref _cleanupRequested, 1, 0) == 0)
            {
                Logger.Debug("Storage cleanup scheduled");
            }
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
