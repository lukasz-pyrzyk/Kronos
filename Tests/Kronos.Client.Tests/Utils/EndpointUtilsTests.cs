using System.Net.Sockets;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client.Utils;
using Xunit;

namespace Kronos.Client.Tests.Utils
{
    public class EndpointUtilsTests
    {
        /// Requires internet connection
        [Theory]
        [InlineData("localhost")]
        [InlineData("google.com")]
        public async Task GetIPAsync_ReturnsIpAddress(string hostName)
        {
            // Act
            var address = await EndpointUtils.GetIpAsync(hostName);

            // Assert
            address.Should().NotBeNull();
            address.AddressFamily.Should().Be(AddressFamily.InterNetwork);
        }
    }
}
