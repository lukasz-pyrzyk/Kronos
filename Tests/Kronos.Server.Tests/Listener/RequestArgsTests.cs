using System.Net.Sockets;
using System.Text;
using Kronos.Core.Requests;
using Kronos.Server.Listening;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class RequestArgsTests
    {
        [Fact]
        public void Reuse_AssignsValues()
        {
            // Arrange
            RequestType type = RequestType.Get;
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");

            // Act
            var message = new RequestArg();
            message.Assign(type, data, data.Length, new Socket(SocketType.Stream, ProtocolType.IP));

            // Assert
            Assert.Equal(message.Type, type);
            Assert.Equal(message.Bytes, data);
            Assert.Equal(message.Count, data.Length);
        }
    }
}
