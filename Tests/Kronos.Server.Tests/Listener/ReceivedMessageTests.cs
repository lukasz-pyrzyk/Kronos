using System.Net.Sockets;
using System.Text;
using Kronos.Core.Requests;
using Kronos.Server.Listening;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class ReceivedMessageTests
    {
        [Fact]
        public void Ctor_AssignsValues()
        {
            // Arrange
            RequestType type = RequestType.Get;
            byte[] data = Encoding.UTF8.GetBytes("lorem ipsum");

            // Act
            var message = new RequestArgs(type, data, data.Length, new Socket(SocketType.Stream, ProtocolType.IP));

            // Assert
            Assert.Equal(message.Type, type);
            Assert.Equal(message.Request, data);
        }
    }
}
