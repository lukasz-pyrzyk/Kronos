using System.Net;
using Kronos.Core.Configuration;
using Xunit;

namespace Kronos.Core.Tests.Configuration
{
    public class NodeConfigurationTests
    {
        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            IPAddress host = IPAddress.Parse("10.10.10.10");
            int port = 11;

            NodeConfiguration configuration = new NodeConfiguration(host, port);

            Assert.Equal(host, configuration.Host);
            Assert.Equal(port, configuration.Port);
            Assert.NotNull(configuration.Endpoint);
        }

        [Fact]
        public void ReturnsCorrectToStringMessage()
        {
            IPAddress host = IPAddress.Parse("10.10.10.10");
            int port = 11;

            NodeConfiguration configuration = new NodeConfiguration(host, port);
            string message = configuration.ToString();

            Assert.Equal(message, $"{host}:{port}");
        }
    }
}
