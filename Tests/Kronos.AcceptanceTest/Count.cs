using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Count : Base
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
            await client.InsertAsync(key, data, DateTime.UtcNow.AddDays(5));
            var countResponse = await client.CountAsync();

            // Assert
            countResponse.Count.Should().Be(1);
        }
    }
}
