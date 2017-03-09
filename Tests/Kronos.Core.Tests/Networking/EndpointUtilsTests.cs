using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Networking;
using Xunit;

namespace Kronos.Core.Tests.Networking
{
    public class EndpointUtilsTests
    {
        [Theory]
        [InlineData("localhost")]
        [InlineData("google.com")]
        public async Task GetIPAsync_ReturnsIpAddress(string hostName)
        {
            // Act
            IPAddress address = await EndpointUtils.GetIPAsync(hostName);

            // Assert
            Assert.NotNull(address);
            Assert.Equal(address.AddressFamily, AddressFamily.InterNetwork);
        }
    }
}
