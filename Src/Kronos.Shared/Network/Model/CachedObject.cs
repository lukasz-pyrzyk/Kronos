using System;

namespace Kronos.Shared.Network.Model
{
    public class CachedObject
    {
        public CachedObject(string key, byte[] objectToCache, DateTime expiryDate)
        {
            Key = key;
            Object = objectToCache;
            ExpiryDate = expiryDate;
        }

        public string Key { get; private set; }
        public byte[] Object { get; private set; }
        public DateTime ExpiryDate { get; private set; }
    }
}
