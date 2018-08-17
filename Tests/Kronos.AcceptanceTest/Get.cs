using System;
using System.Threading.Tasks;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Get : Base
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
            new Random().NextBytes(data);

            // Act
            await client.InsertAsync(key, data, DateTimeOffset.UtcNow.AddDays(5));
            byte[] received = await client.GetAsync(key);

            // Assert
            Assert.Equal(data, received);
        }
    }
}
