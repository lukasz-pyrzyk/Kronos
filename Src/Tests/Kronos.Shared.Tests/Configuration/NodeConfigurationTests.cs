using Kronos.Shared.Configuration;
using Kronos.Tests.Helpers;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Shared.Tests.Configuration
{
    public class NodeConfigurationTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            string host = _fixture.CreateIpAddress();
            int port = _fixture.Create<int>();

            NodeConfiguration configuration = new NodeConfiguration(host, port);

            Assert.Equal(host, configuration.Host);
            Assert.Equal(port, configuration.Port);
            Assert.NotNull(configuration.Endpoint);
        }

        [Fact]
        public void ReturnsCorrectToStringMessage()
        {
            string host = _fixture.CreateIpAddress();
            int port = _fixture.Create<int>();

            NodeConfiguration configuration = new NodeConfiguration(host, port);
            string message = configuration.ToString();

            Assert.Equal(message, $"{host}:{port}");
        }
    }
}
