using System.Net.Sockets;
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
            var request = new Request();
            var socket = new Socket(SocketType.Stream, ProtocolType.IP);

            // Act
            var message = new RequestArg(request, socket);

            // Assert
            Assert.Equal(message.Request, request);
            Assert.NotNull(message.Client);
        }
    }
}
