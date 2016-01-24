using Kronos.Core.Model;

namespace Kronos.Core.Requests
{
    public class InsertRequest : Request
    {
        public CachedObject ObjectToCache { get; }
        
        public InsertRequest(CachedObject objectToCache)
        {
            ObjectToCache = objectToCache;
        }
        
        public override byte[] Serialize()
        {
            byte[] keyPackage = Serialize(ObjectToCache.Key);
            byte[] objectPackage = ObjectToCache.Object;
            byte[] expiryDate = Serialize(ObjectToCache.ExpiryDate);
            byte[] totalSize = GetTotalSize(keyPackage.Length + objectPackage.Length + expiryDate.Length);

            // TODO Connect totalSize with Join method. Maybe JointWithTotalSize?
            return Join(totalSize, keyPackage, objectPackage, expiryDate);
        }
    }
}
