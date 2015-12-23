using System.Collections.Generic;
using System.IO;
using Kronos.Shared.Configuration;
using Kronos.Tests.Helpers;
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
            NodeConfiguration nodeConfiguraton = new NodeConfiguration(_fixture.CreateIpAddress(), _fixture.Create<int>());
            ServerConfiguration configuration = _fixture.Build<ServerConfiguration>().With(x => x.NodesConfiguration, new List<NodeConfiguration>() { nodeConfiguraton }).Create();

            NodeConfiguration nodeConfiguration = configuration.GetNodeForStream(Arg.Any<Stream>());

            Assert.NotNull(nodeConfiguration);
        }
    }
}
