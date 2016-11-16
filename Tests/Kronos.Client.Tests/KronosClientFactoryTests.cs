using Xunit;

namespace Kronos.Client.Tests
{
    public class KronosClientFactoryTests
    {
        [Fact]
        public void CreateClient_FromIpAndPort()
        {
            // Arrange
            const string ip = "8.8.8.8";
            const int port = 500;

            // Act
            IKronosClient client = KronosClientFactory.CreateClientFromIp(ip, port);

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public void CreateClient_FromDomain()
        {
            // Arrange
            const string localHost = "localhost";
            const int port = 500;

            // Act
            IKronosClient client = KronosClientFactory.CreateClient(localHost, port);

            // Assert
            Assert.NotNull(client);
        }
    }
}
