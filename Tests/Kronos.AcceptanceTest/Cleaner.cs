using System;
using System.Threading.Tasks;
using Kronos.Client;
using Kronos.Core.Storage;
using Kronos.Core.Storage.Cleaning;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Cleaner : Base
    {
        [Fact]
        public override async Task RunAsync()
        {
            await RunInternalAsync();
        }

        protected override async Task ProcessAsync(IKronosClient client)
        {
            // Arrange
            string key = Guid.NewGuid().ToString();
            byte[] data = new byte[1024];

            // Act
            await client.InsertAsync(key, data, DateTime.UtcNow);

            await Task.Delay(Scheduler.DefaultPeriod);

            bool contains = await client.ContainsAsync(key);

            // Assert
            Assert.False(contains, "Object exists after cleanup");
        }
    }
}
