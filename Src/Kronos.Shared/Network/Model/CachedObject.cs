using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kronos.Shared.Network.Model
{
    [Serializable]
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

        public byte[] SerializeNetworkPackage()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static CachedObject DeserializeNetworkPackage(byte[] package)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(package, 0, package.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                CachedObject obj = (CachedObject)binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
