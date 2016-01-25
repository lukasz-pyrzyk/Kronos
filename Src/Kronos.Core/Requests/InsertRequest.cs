using System;
using System.Runtime.InteropServices;
using System.Text;
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
            byte[] keyPackageSize = Serialize(keyPackage.Length);

            byte[] objectPackage = ObjectToCache.Object;
            byte[] objectPackageSize = Serialize(objectPackage.Length);

            byte[] expiryDate = Serialize(ObjectToCache.ExpiryDate);
            byte[] expiryDateSize = Serialize(expiryDate.Length);

            return JoinWithTotalSize(keyPackageSize, keyPackage, objectPackageSize, objectPackage, expiryDateSize, expiryDate);
        }

        public static InsertRequest Deserialize(byte[] stream)
        {
            int offset = sizeof(int); // skip total size of package

            byte[] keyPackageSizeBytes = GetPartOfStream<int>(stream, ref offset);
            int keyPackageSize = DeserializeInt(keyPackageSizeBytes);

            byte[] keyBytes = GetPartOfStream(stream, ref offset, keyPackageSize);
            string key = DeserializeString(keyBytes);

            byte[] objSizeBytes = GetPartOfStream<int>(stream, ref offset);
            int objSize = DeserializeInt(objSizeBytes);

            byte[] obj = GetPartOfStream(stream, ref offset, objSize);

            byte[] expiryDateSizeBytes = GetPartOfStream<int>(stream, ref offset);
            int expiryDateSize = DeserializeInt(expiryDateSizeBytes);

            byte[] expiryDateBytes = GetPartOfStream(stream, ref offset, expiryDateSize);

            DateTime expiryDate = DeseriazeDatetime(expiryDateBytes);
            CachedObject cachedObject = new CachedObject(key, obj, expiryDate);

            return new InsertRequest(cachedObject);
        }
    }
}
