using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Authorization : Base
    {
        [Fact]
        public override async Task RunAsync()
        {
            await RunInternalAsync();
        }

        protected override async Task ProcessAsync(IKronosClient client)
        {
            // Act
            var countResponse = await client.CountAsync();

            countResponse.Should().BeNull();
        }

        protected override string[] GetSettings()
        {
            return new[] { "--password", Guid.NewGuid().ToString() };
        }
    }
}
