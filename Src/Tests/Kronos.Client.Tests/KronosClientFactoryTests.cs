using System.Net;
using Kronos.Core.Configuration;
using Xunit;

namespace Kronos.Client.Tests.Core
{
    public class KronosClientFactoryTests
    {
        [Fact]
        public void CanCreateIKronosClient()
        {
            IKronosClient client = KronosClientFactory.CreateClient("KronosConfig.json");

            Assert.NotNull(client);
        }
    }
}

