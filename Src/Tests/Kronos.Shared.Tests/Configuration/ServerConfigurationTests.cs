using System.IO;
using Kronos.Shared.Configuration;
using NSubstitute;
using Ploeh.AutoFixture;
using Xunit;

namespace Kronos.Shared.Tests.Configuration
{
    public class ServerConfigurationTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanGetNodesConfiguration()
        {
            ServerConfiguration configuration = _fixture.Create<ServerConfiguration>();

            NodeConfiguration nodeConfiguration = configuration.GetNodeForStream(Arg.Any<Stream>());

            Assert.NotNull(nodeConfiguration);
        }
    }
}
