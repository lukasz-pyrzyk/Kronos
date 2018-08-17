using System;
using System.Threading.Tasks;
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
            new Random().NextBytes(data);

            // Act
            bool added = await client.InsertAsync(key, data, DateTimeOffset.UtcNow.AddDays(5));
            bool contains = await client.ContainsAsync(key);

            // Assert
            Assert.True(added, "Object is not added");
            Assert.True(contains, "Object is not added, contains returns false");
        }
    }
}
