using System;
using ProtoBuf;

namespace Kronos.Core.Model
{
    [ProtoContract]
    public class CachedObject
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public byte[] Object { get; set; }

        [ProtoMember(3)]
        public DateTime ExpiryDate { get; set; }

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
    }
}
