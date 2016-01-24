using System;
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
            int offset = 0;
            byte[] totalPackageSizeBytes = new byte[sizeof(int)];
            Array.Copy(stream, totalPackageSizeBytes, sizeof(int));
            offset += sizeof(int);

            byte[] keyPackageSizeBytes = new byte[sizeof(int)];
            Array.ConstrainedCopy(stream, offset, keyPackageSizeBytes, 0, keyPackageSizeBytes.Length);
            int keyPackageSize = BitConverter.ToInt32(keyPackageSizeBytes, 0);
            offset += sizeof(int);

            byte[] keyBytes = new byte[keyPackageSize];
            Array.ConstrainedCopy(stream, offset, keyBytes, 0, keyBytes.Length);
            string key = Encoding.UTF8.GetString(keyBytes);
            offset += keyBytes.Length;

            byte[] objSizeBytes = new byte[sizeof(int)];
            Array.ConstrainedCopy(stream, offset, objSizeBytes, 0, objSizeBytes.Length);
            int objSize = BitConverter.ToInt32(objSizeBytes, 0);
            offset += sizeof(int);

            byte[] obj = new byte[objSize];
            Array.ConstrainedCopy(stream, offset, obj, 0, obj.Length);
            offset += objSize;

            byte[] expiryDateSizeBytes = new byte[sizeof(int)];
            Array.ConstrainedCopy(stream, offset, expiryDateSizeBytes, 0, expiryDateSizeBytes.Length);
            int expiryDateSize = BitConverter.ToInt32(expiryDateSizeBytes, 0);
            offset += sizeof(int);

            byte[] expiryDateBytes = new byte[expiryDateSize];
            Array.ConstrainedCopy(stream, offset, expiryDateBytes, 0, expiryDateBytes.Length);
            long ticks = BitConverter.ToInt64(expiryDateBytes, 0);

            DateTime expiryDate = DateTime.FromBinary(ticks);

            CachedObject cachedObject = new CachedObject(key, obj, expiryDate);

            return new InsertRequest(cachedObject);
        }
    }
}
