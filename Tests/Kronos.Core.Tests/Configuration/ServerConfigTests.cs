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
            const string ip = "127.0.0.1";
            const int port = 5000;

            ServerConfig config = new ServerConfig
            {
                Ip = ip,
                Port = port
            };

            Assert.Equal(config.EndPoint.Address, IPAddress.Parse(ip));
            Assert.Equal(config.EndPoint.Port, port);
        }
    }
}
