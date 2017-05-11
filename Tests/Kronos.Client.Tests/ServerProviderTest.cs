using System.Linq;
using Kronos.Core.Configuration;
using Xunit;

namespace Kronos.Client.Tests
{
    public class ServerProviderTest
    {
        [Fact]
        public void Ctor_AssingsClusterConfig()
        {
            ClusterConfig config = new ClusterConfig { Servers = CreateServers(2) };
            ServerProvider provider = new ServerProvider(config);

            Assert.NotNull(provider);
            Assert.Equal(provider.Servers.Length, config.Servers.Length);
            Assert.Equal(provider.Servers, config.Servers);
        }

        [Fact]
        public void Ctor_CreatesMappingTable_WhereModuloIsEmpty()
        {
            ServerConfig[] servers = CreateServers(2);
            ClusterConfig config = new ClusterConfig { Servers = servers };

            int rangePerServer = 100 / config.Servers.Length;

            ServerProvider provider = new ServerProvider(config);

            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(servers[0].EndPoint)), rangePerServer);
            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(servers[1].EndPoint)), rangePerServer);
        }

        [Fact]
        public void Ctor_CreatesMappingTable_WhereModuloIsNotEmpty()
        {
            ServerConfig[] servers = CreateServers(3);
            ClusterConfig config = new ClusterConfig { Servers = servers };

            int modulo = 100 % config.Servers.Length;
            int rangePerServer = 100 / config.Servers.Length;

            ServerProvider provider = new ServerProvider(config);

            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(servers[0].EndPoint)), rangePerServer + modulo);
            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(servers[1].EndPoint)), rangePerServer);
            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(servers[2].EndPoint)), rangePerServer);
        }

        [Fact]
        public void SelectServer_ReturnsServerFroMappingTable()
        {
            ServerConfig[] servers = CreateServers(1);
            ClusterConfig config = new ClusterConfig { Servers = servers };

            ServerProvider provider = new ServerProvider(config);
            ServerConfig selectedServer = provider.SelectServer(GetHashCode());

            Assert.Equal(selectedServer, servers[0]);
        }
        
        private static ServerConfig[] CreateServers(int count)
        {
            var servers = Enumerable.Range(0, count)
                .Select(x => new ServerConfig
                {
                    Ip = Settings.LocalIp,
                    Port = Settings.DefaultPort + x
                }).ToArray();

            return servers;
        }
    }
}
