using System.Net;
using Xunit;

namespace Kronos.Client.Tests.Core
{
    public class KronosClientFactoryTests
    {
        [Fact]
        public void CanCreateIKronosClient()
        {
            IKronosClient client = KronosClientFactory.CreateClient(IPAddress.Any, 5000);

            Assert.NotNull(client);
        }
    }
}
