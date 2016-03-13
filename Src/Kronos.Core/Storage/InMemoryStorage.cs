using System.Collections.Concurrent;
using NLog;

namespace Kronos.Core.Storage
{
    public class InMemoryStorage : IStorage
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<string, byte[]> _storage = new ConcurrentDictionary<string, byte[]>();

        public int Count => _storage.Count;

        public void AddOrUpdate(string key, byte[] obj)
        {
            _logger.Debug($"Inserting key {key} to MemoryStorage");
            _storage.AddOrUpdate(key, obj, (s, bytes) => obj);
        }

        public byte[] TryGet(string key)
        {
            _logger.Debug($"Getting package for key {key}");
            byte[] obj;
            if (_storage.TryGetValue(key, out obj))
            {
                _logger.Debug($"Returning object of key {key}");
                return obj;
            }

            _logger.Debug($"Key {key} not found. Returning null");
            return null;
        }

        public void Clear()
        {
            _logger.Info("Clearing InMemoryCache");
            _storage.Clear();
        }
    }
}
