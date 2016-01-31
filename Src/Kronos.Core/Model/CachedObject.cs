using System;

namespace Kronos.Core.Model
{
    public class CachedObject
    {
        // used by reflection
        public CachedObject()
        {
        }

        public CachedObject(string key, byte[] objectToCache, DateTime expiryDate)
        {
            Key = key;
            Object = objectToCache;
            ExpiryDate = expiryDate;
        }

        public string Key { get; set; }
        public byte[] Object { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
