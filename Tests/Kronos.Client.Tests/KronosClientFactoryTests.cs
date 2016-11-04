using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace Kronos.Client.Tests
{
    public class KronosClientFactoryTests
    {

        [Fact]
        public void CreateClient_FromIpAndPort()
        {
            // Arrange
            string ip = "8.8.8.8";
            int port = 500;

            // Act
            IKronosClient client = KronosClientFactory.CreateClient(ip, port);

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public void CreateClient_FromPort()
        {
            // Arrange
            int port = 500;

            // Act
            IKronosClient client = KronosClientFactory.CreateClient(port);

            // Assert
            Assert.NotNull(client);
        }

        private static async Task<string> GetLocalIp()
        {
            var hosts = await Dns.GetHostAddressesAsync(Dns.GetHostName());

            string localIp = hosts.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();

            return localIp;
        }
    }
}
