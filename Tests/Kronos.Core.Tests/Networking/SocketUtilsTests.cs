using System.Net.Sockets;
using Kronos.Core.Networking;
using Xunit;

namespace Kronos.Core.Tests.Networking
{
    public class SocketUtilsTests
    {
        [Fact]
        public void Setup_ConfiguresTimeouts()
        {
            // Arrange
            var socket = new Socket(SocketType.Stream, ProtocolType.IP);

            // Act
            SocketUtils.Prepare(socket);

            // Assert
            Assert.Equal(SocketUtils.Timeout, socket.ReceiveTimeout);
            Assert.Equal(SocketUtils.Timeout, socket.SendTimeout);
        }

        //[Fact()]]
        public void Setup_ConfiguresBuffers()
        {
            // Arrange
            var socket = new Socket(SocketType.Stream, ProtocolType.IP);

            // Act
            SocketUtils.Prepare(socket);

            // Assert
            Assert.Equal(SocketUtils.BufferSize, socket.ReceiveTimeout);
            Assert.Equal(SocketUtils.BufferSize, socket.SendBufferSize);
        }
    }
}
