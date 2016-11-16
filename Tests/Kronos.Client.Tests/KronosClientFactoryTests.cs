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
            IKronosClient client = KronosClientFactory.CreateClient(port, ip);

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
    }
}
