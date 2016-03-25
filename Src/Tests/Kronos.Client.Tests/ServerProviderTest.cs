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
            ClusterConfig config = new ClusterConfig { Servers = new[] { new ServerConfig(), new ServerConfig() } };
            ServerProvider provider = new ServerProvider(config);

            Assert.NotNull(provider);
            Assert.Equal(provider.ServersCount, config.Servers.Length);
        }

        [Fact]
        public void Ctor_CreatesMappingTable_WhereModuloIsEmpty()
        {
            var first = new ServerConfig
            {
                Ip = "192.168.0.1",
                Port = 5000
            };

            var second = new ServerConfig
            {
                Ip = "192.168.0.1",
                Port = 5001
            };


            ClusterConfig config = new ClusterConfig
            {
                Servers = new[] { first, second }
            };

            int rangePerServer = 100/config.Servers.Length;

            ServerProvider provider = new ServerProvider(config);

            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(first.EndPoint)), rangePerServer);
            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(second.EndPoint)), rangePerServer);
        }

        [Fact]
        public void Ctor_CreatesMappingTable_WhereModuloIsNotEmpty()
        {
            var first = new ServerConfig
            {
                Ip = "192.168.0.1",
                Port = 5000
            };

            var second = new ServerConfig
            {
                Ip = "192.168.0.1",
                Port = 5001
            };

            var three = new ServerConfig
            {
                Ip = "192.168.0.1",
                Port = 5002
            };
            
            ClusterConfig config = new ClusterConfig
            {
                Servers = new[] { first, second, three }
            };

            int modulo = 100 % config.Servers.Length;
            int rangePerServer = 100 / config.Servers.Length;

            ServerProvider provider = new ServerProvider(config);

            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(first.EndPoint)), rangePerServer + modulo);
            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(second.EndPoint)), rangePerServer);
            Assert.Equal(provider.Mappings.Values.Count(x => x.EndPoint.Equals(three.EndPoint)), rangePerServer);
        }

        [Fact]
        public void SelectServer_ReturnsServerFroMappingTable()
        {
            var first = new ServerConfig
            {
                Ip = "192.168.0.1",
                Port = 5000
            };

            ClusterConfig config = new ClusterConfig
            {
                Servers = new[] { first}
            };
            

            ServerProvider provider = new ServerProvider(config);
            ServerConfig selectedServer =  provider.SelectServer(GetHashCode());

            Assert.Equal(selectedServer, first);
        }

    }
}
