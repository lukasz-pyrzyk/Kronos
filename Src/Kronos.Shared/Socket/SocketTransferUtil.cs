using System;
using System.Linq;
using System.Text;
using Kronos.Shared.Network.Model;

namespace Kronos.Shared.Socket
{
    public class SocketTransferUtil
    {
        public static byte[] GetTotalBytes(CachedObject cachedObject)
        {
            byte[] keyPackage = Encoding.UTF8.GetBytes(cachedObject.Key);
            byte[] objectPackage = cachedObject.Object;
            byte[] expiryDate = BitConverter.GetBytes(cachedObject.ExpiryDate.Ticks);
            byte[] totalSize = BitConverter.GetBytes(sizeof(int) + keyPackage.Length + objectPackage.Length + expiryDate.Length);

            return Join(totalSize, keyPackage, objectPackage, expiryDate);
        }

        public static byte[] Join(params byte[][] arrays)
        {
            byte[] finalArray = new byte[arrays.Sum(x => x.Length)];

            int offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, finalArray, offset, data.Length);
                offset += data.Length;
            }

            return finalArray;
        }
    }
}
