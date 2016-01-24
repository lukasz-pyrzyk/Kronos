using System;
using System.Linq;
using System.Text;

namespace Kronos.Core.Requests
{
    /// <summary>
    /// Base class for any request to the server
    /// </summary>
    public abstract class Request
    {
        public abstract byte[] Serialize();

        protected byte[] JoinWithTotalSize(params byte[][] arrays)
        {
            int packagesSize = arrays.Sum(x => x.Length);
            byte[] packageSizeBytes = Serialize(packagesSize);

            int intSize = sizeof(int);
            byte[] finalArray = new byte[intSize + packagesSize];

            Buffer.BlockCopy(packageSizeBytes, 0, finalArray, 0, packageSizeBytes.Length);

            int offset = intSize;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, finalArray, offset, data.Length);
                offset += data.Length;
            }

            return finalArray;
        }

        protected byte[] Serialize(int value)
        {
            return BitConverter.GetBytes(value);
        }

        protected byte[] Serialize(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        protected byte[] Serialize(DateTime value)
        {
            return BitConverter.GetBytes(value.Ticks);
        }
    }
}
