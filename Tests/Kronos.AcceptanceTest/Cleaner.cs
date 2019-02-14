using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client;
using Kronos.Core.Configuration;
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

            await Task.Delay(Settings.CleanupTimeMs);

            var containsResponse = await client.ContainsAsync(key);

            // Assert
            containsResponse.Contains.Should().BeFalse("Object should be deleted because of the cleanup");
        }
    }
}
