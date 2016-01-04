using System.Linq;
using Kronos.Shared.Socket;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Shared.Tests.Socket
{
    public class SocketTransferUtilTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanAddTwoByteArrays()
        {
            byte[][] arrays = _fixture.Create<byte[][]>();

            byte[] splited = SocketTransferUtil.Split(arrays);

            Assert.Equal(splited.Length, arrays.Sum(x => x.Length));

            int lastPointer = 0;
            foreach (byte[] array in arrays)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    Assert.Equal(splited[lastPointer + j], array[j]);
                }

                lastPointer += array.Length;
            }
        }
    }
}
