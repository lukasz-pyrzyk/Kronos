using System.Net;
using Kronos.Core.Configuration;
using Xunit;

namespace Kronos.Core.Tests.Configuration
{
    public class ServerConfigTests
    {
        [Fact]
        public void EndPoint_ContainsCorrectIPAndPort()
        {
            // Arrange
            const string ip = "127.0.0.1";
            const int port = 5000;

            // Act
            ServerConfig config = new ServerConfig
            {
                Ip = ip,
                Port = port
            };

            // Assert
            Assert.Equal(config.EndPoint.Address, IPAddress.Parse(ip));
            Assert.Equal(config.EndPoint.Port, port);
        }

        [Fact]
        public void ToString_ReturnsEndpoint()
        {
            // Arrange
            const string ip = "192.168.0.1";
            const int port = 5000;

            // Act
            ServerConfig config = new ServerConfig
            {
                Ip = ip,
                Port = port
            };

            // Assert
            Assert.Equal(config.EndPoint.ToString(), config.ToString());
        }
    }
}
