using System.Collections.Concurrent;
using NLog;

namespace Kronos.Server.Storage
{
    public class InMemoryStorage
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static readonly ConcurrentDictionary<string, byte[]> _storage = new ConcurrentDictionary<string, byte[]>();

        public static int Count => _storage.Count;

        public static void AddOrUpdate(string key, byte[] obj)
        {
            _logger.Debug($"Inserting key {key} to MemoryStorage");
            _storage.AddOrUpdate(key, obj, (s, bytes) => obj);
        }

        public static byte[] TryGet(string key)
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

        public static void Clear()
        {
            _logger.Debug("Clearing InMemoryCache");
            _storage.Clear();
        }
    }
}
