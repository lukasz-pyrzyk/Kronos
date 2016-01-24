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

        protected byte[] GetTotalSize(params int[] values)
        {
            int totalSize = values.Sum() + sizeof(int);
            return BitConverter.GetBytes(totalSize);
        }

        protected byte[] Join(params byte[][] arrays)
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

        protected byte[] Serialize(string word)
        {
            return Encoding.UTF8.GetBytes(word);
        }

        protected byte[] Serialize(DateTime datetime)
        {
            return BitConverter.GetBytes(datetime.Ticks);
        }
    }
}
