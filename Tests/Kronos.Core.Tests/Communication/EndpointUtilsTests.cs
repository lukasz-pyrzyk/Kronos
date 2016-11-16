using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Xunit;

namespace Kronos.Core.Tests.Communication
{
    public class EndpointUtilsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("localhost")]
        [InlineData("google.com")]
        public async Task GetIPAsync_ReturnsIpAddress(string hostName)
        {
            // Act
            IPAddress address = await EndpointUtils.GetIPAsync();

            // Assert
            Assert.NotNull(address);
            Assert.Equal(address.AddressFamily, AddressFamily.InterNetwork);
        }
    }
}
