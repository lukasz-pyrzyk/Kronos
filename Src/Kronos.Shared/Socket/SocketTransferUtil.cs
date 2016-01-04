using System;
using System.Linq;

namespace Kronos.Shared.Socket
{
    public class SocketTransferUtil
    {
        public static byte[] Split(params byte[][] arrays)
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
