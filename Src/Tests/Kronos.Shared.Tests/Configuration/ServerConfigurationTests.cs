using System.Collections.Generic;
using Kronos.Shared.Configuration;
using Kronos.Shared.Network.Model;
using Kronos.Tests.Helpers;
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
            CachedObject cachedObject = _fixture.Create<CachedObject>();

            NodeConfiguration nodeConfiguration = configuration.GetNodeForStream(cachedObject);

            Assert.NotNull(nodeConfiguration);
        }
    }
}
