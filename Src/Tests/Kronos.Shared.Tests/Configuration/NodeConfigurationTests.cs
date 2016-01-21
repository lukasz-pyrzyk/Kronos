using Kronos.Shared.Configuration;
using Xunit;

namespace Kronos.Shared.Tests.Configuration
{
    public class NodeConfigurationTests
    {
        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            string host = "10.10.10.10";
            int port = 11;

            NodeConfiguration configuration = new NodeConfiguration(host, port);

            Assert.Equal(host, configuration.Host);
            Assert.Equal(port, configuration.Port);
            Assert.NotNull(configuration.Endpoint);
        }

        [Fact]
        public void ReturnsCorrectToStringMessage()
        {
            string host = "10.10.10.10";
            int port = 11;

            NodeConfiguration configuration = new NodeConfiguration(host, port);
            string message = configuration.ToString();

            Assert.Equal(message, $"{host}:{port}");
        }
    }
}
