using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Stats : Base
    {
        [Fact]
        public override async Task RunAsync()
        {
            await RunInternalAsync();
        }

        protected override async Task ProcessAsync(IKronosClient client)
        {
            // Arrange
            var elements = 5;
            var mbperElement = 5;
            var data = new byte[mbperElement * 1024 * 1024];
            for (int i = 0; i < elements; i++)
            {
                await client.InsertAsync(Guid.NewGuid().ToString(), data, null);
            }

            // Act
            var stats = await client.StatsAsync();

            // Assert
            stats.Should().NotBeNull();
            stats.Elements.Should().Be(elements);
            stats.MemoryUsed.Should().Be(elements * mbperElement);
        }
    }
}
