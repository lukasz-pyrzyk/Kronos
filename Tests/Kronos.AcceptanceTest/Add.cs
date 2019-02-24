using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Add : Base
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
            var addedResponse = await client.InsertAsync(key, data, DateTimeOffset.UtcNow.AddDays(5));
            var containsResponse = await client.ContainsAsync(key);

            // Assert
            addedResponse.Added.Should().BeTrue();
            containsResponse.Contains.Should().BeTrue();
        }
    }
}
