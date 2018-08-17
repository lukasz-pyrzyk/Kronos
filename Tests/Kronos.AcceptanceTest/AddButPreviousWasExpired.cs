using System;
using System.Threading.Tasks;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class AddButPreviousWasExpired : Base
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
            TimeSpan expiryTime = TimeSpan.FromSeconds(1);
            var now = DateTimeOffset.UtcNow;

            // Act
            bool added = await client.InsertAsync(key, data, now + expiryTime);
            await Task.Delay(expiryTime);
            bool addedAgain = await client.InsertAsync(key, data, now + TimeSpan.FromDays(1));

            // Assert
            Assert.True(added, "Object was not added");
            Assert.True(addedAgain, "Object is not added even if previous was expired");
        }
    }
}
