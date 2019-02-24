using System;
using System.Threading.Tasks;
using FluentAssertions;
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
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Act
            var addedResponse = await client.InsertAsync(key, data, now + expiryTime);
            await Task.Delay(expiryTime);
            var addedAgainResponse = await client.InsertAsync(key, data, now + TimeSpan.FromDays(1));

            // Assert
            addedResponse.Added.Should().BeTrue();
            addedAgainResponse.Added.Should().BeTrue("Object is not added even if previous was expired");
        }
    }
}
