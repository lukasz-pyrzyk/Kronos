using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Kronos.Server.Storage
{
    class InMemoryStorage
    {
        private readonly Dictionary<Key, Element> _storage = new Dictionary<Key, Element>();
        private readonly PriorityQueue<ExpiringKey> _expiringKeys = new PriorityQueue<ExpiringKey>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private readonly ILogger<InMemoryStorage> _logger;

        public InMemoryStorage(ILogger<InMemoryStorage> logger)
        {
            _logger = logger;
        }

        public int Count => _storage.Count;

        public int ExpiringCount => _expiringKeys.Count;

        public bool Add(string name, DateTimeOffset? expiryDate, ByteString obj)
        {
            var key = new Key(name);

            _lock.EnterUpgradeableReadLock();
            try
            {
                bool found = _storage.TryGetValue(key, out Element element);
                if (found && !element.IsExpired())
                {
                    return false;
                }
                
                _lock.EnterWriteLock();
                try
                {
                    element = new Element(obj, expiryDate);
                    _storage[key] = element;

                    if (expiryDate.HasValue)
                    {
                        _expiringKeys.Add(new ExpiringKey(key, expiryDate.Value));
                    }

                    return true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public bool TryGet(string name, out ByteString obj)
        {
            var key = new Key(name);
            _lock.EnterReadLock();
            try
            {
                bool found = _storage.TryGetValue(key, out Element element);
                if (found && !element.IsExpired())
                {
                    obj = element.Data;
                    return true;
                }

                obj = null;
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool TryRemove(string name)
        {
            var key = new Key(name);
            _lock.EnterUpgradeableReadLock();
            try
            {
                bool found = _storage.TryGetValue(key, out Element element);
                if (!found) return false;

                _lock.EnterWriteLock();
                try
                {
                    _storage.Remove(key);
                    if (element.IsExpiring)
                    {
                        _expiringKeys.Remove(new ExpiringKey(key, default));
                    }
                    return true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public bool Contains(string name)
        {
            var key = new Key(name);

            _lock.EnterReadLock();
            try
            {
                bool found = _storage.TryGetValue(key, out Element element);
                return found && !element.IsExpired();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public int Clear()
        {
            _logger.LogInformation("Clearing storage");

            _lock.EnterWriteLock();
            try
            {
                int count = Count;
                _storage.Clear();
                _expiringKeys.Clear();

                return count;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Cleanup()
        {
            DateTimeOffset date = DateTimeOffset.UtcNow;
            var deleted = 0;

            _lock.EnterWriteLock();
            try
            {
                while (_expiringKeys.Count > 0 && _expiringKeys.Peek().IsExpired(date))
                {
                    ExpiringKey expiringKey = _expiringKeys.Poll();
                    _storage.Remove(expiringKey.Key);
                    deleted++;
                    if (deleted > 0)
                    {
                        _logger.LogDebug("Deleted {deleted} elements from storage", deleted);
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public Element[] GetAll()
        {
            _lock.EnterReadLock();
            try
            {
                return _storage.Values.ToArray();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
