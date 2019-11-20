using System;
using System.Collections.Generic;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Kronos.Server.Storage
{
    class InMemoryStorage
    {
        private readonly Dictionary<Key, Element> _storage = new Dictionary<Key, Element>();
        private readonly ConcurrentPriorityQueue<ExpiringKey> _expiringKeys = new ConcurrentPriorityQueue<ExpiringKey>();

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

            bool found = _storage.TryGetValue(key, out Element element);
            if (found && !element.IsExpired())
            {
                return false;
            }

            element = new Element(obj, expiryDate);
            _storage[key] = element;

            if (expiryDate.HasValue)
            {
                _expiringKeys.Add(new ExpiringKey(key, expiryDate.Value));
            }

            return true;
        }

        public bool TryGet(string name, out ByteString obj)
        {
            var key = new Key(name);
            bool found = _storage.TryGetValue(key, out Element element);
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
            var key = new Key(name);
            bool found = _storage.TryGetValue(key, out Element element);
            if (found)
            {
                _storage.Remove(key);
                if (element.IsExpiring)
                {
                    _expiringKeys.Remove(new ExpiringKey(key, default));
                }
            }

            return found;
        }

        public bool Contains(string name)
        {
            var key = new Key(name);
            bool found = _storage.TryGetValue(key, out Element element);

            return found && !element.IsExpired();
        }

        public int Clear()
        {
            _logger.LogInformation("Clearing storage");

            int count = Count;
            _storage.Clear();
            _expiringKeys.Clear();

            return count;
        }

        public void Cleanup()
        {
            DateTimeOffset date = DateTimeOffset.UtcNow;
            var deleted = 0;

            while (_expiringKeys.Count > 0 && _expiringKeys.Peek().IsExpired(date))
            {
                ExpiringKey expiringKey = _expiringKeys.Poll();
                _storage.Remove(expiringKey.Key);
                deleted++;
            }

            if (deleted > 0)
            {
                _logger.LogDebug("Deleted {deleted} elements from storage", deleted);
            }
        }

        public IEnumerable<Element> GetAll() => _storage.Values;
    }
}
