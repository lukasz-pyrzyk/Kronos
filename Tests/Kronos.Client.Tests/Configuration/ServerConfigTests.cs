using System.Net;
using FluentAssertions;
using Kronos.Client.Configuration;
using Xunit;

namespace Kronos.Client.Tests.Configuration
{
    public class ServerConfigTests
    {
        [Fact]
        public void EndPoint_ContainsCorrectIPAndPort()
        {
            // Arrange && act
            const string ip = "127.0.0.1";
            const int port = 44000;

            // Act
            var config = new ServerConfig
            {
                Ip = ip,
                Port = port
            };

            // Assert
            config.EndPoint.Address.Should().Be(IPAddress.Parse(ip));
            config.EndPoint.Port.Should().Be(port);
            config.EndPoint.ToString().Should().Be(config.ToString());
        }
    }
}
