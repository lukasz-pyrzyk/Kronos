using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kronos.Shared.Socket;
using Xunit;

namespace Kronos.Shared.Tests.Socket
{
    public class SocketTransferUtilTests
    {
        [Fact]
        public void CanAddTwoByteArrays()
        {
            ICollection<byte[]> arrays = new List<byte[]>
            {
                Encoding.UTF8.GetBytes("key"),
                new byte[5],
                BitConverter.GetBytes(DateTime.MaxValue.Ticks)
            };

            byte[] total = SocketTransferUtil.Join(arrays.ToArray());

            Assert.Equal(total.Length, arrays.Sum(x => x.Length));

            int offset = 0;
            foreach (byte[] array in arrays)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    Assert.Equal(total[offset + j], array[j]);
                }

                offset += array.Length;
            }
        }
    }
}
