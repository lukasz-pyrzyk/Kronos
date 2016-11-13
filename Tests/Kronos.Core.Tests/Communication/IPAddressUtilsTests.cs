using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Xunit;

namespace Kronos.Core.Tests.Communication
{
    public class IPAddressUtilsTests
    {
        [Fact]
        public async Task GetLocalAsync_ReturnsIpAddress()
        {
            // Act
            IPAddress address = await IPAddressUtils.GetLocalAsync();

            // Assert
            Assert.NotNull(address);
            Assert.Equal(address.AddressFamily, AddressFamily.InterNetwork);
        }
    }
}
