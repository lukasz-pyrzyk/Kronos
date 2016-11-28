using System.Net.Sockets;
using System.Text;
using Kronos.Core.Requests;
using Kronos.Server.Listener;
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
            ReceivedMessage message = new ReceivedMessage(new Socket(SocketType.Stream, ProtocolType.IP), type, data, data.Length);

            // Assert
            Assert.Equal(message.Type, type);
            Assert.Equal(message.Buffer, data);
        }
    }
}
