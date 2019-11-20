using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Cleaner_RemovesElement : Base
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
            var insertResponse = await client.InsertAsync(key, data, DateTimeOffset.UtcNow);
            insertResponse.Added.Should().BeTrue();

            await Task.Delay(5000);

            var containsResponse = await client.ContainsAsync(key);

            // Assert
            containsResponse.Contains.Should().BeFalse("Object should be deleted because of the cleanup");
        }
    }
}
