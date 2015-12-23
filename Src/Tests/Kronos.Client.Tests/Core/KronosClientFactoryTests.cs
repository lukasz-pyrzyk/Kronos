using Kronos.Client.Core;
using Xunit;

namespace Kronos.Client.Tests.Core
{
    public class KronosClientFactoryTests
    {
        [Fact]
        public void CanCreateIKronosClient()
        {
            IKronosClient client = KronosClientFactory.CreateClient();

            Assert.NotNull(client);
        }
    }
}
