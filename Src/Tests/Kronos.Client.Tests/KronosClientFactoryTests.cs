using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Kronos.Client.Tests
{
    public class KronosClientFactoryTests
    {
        [Fact]
        public void CanCreateIKronosClient()
        {
            string dir = PlatformServices.Default.Application.ApplicationBasePath;
            IKronosClient client = KronosClientFactory.CreateClient($"{dir}\\KronosConfig.json");

            Assert.NotNull(client);
        }
    }
}