using System;
using System.Threading.Tasks;
using Kronos.Client;
using Xunit;
using Xunit.Abstractions;

namespace Kronos.AcceptanceTest
{
    public class Count : Base
    {
        public Count(ITestOutputHelper output) : base(output)
        {
        }

        protected override async Task ProcessAsync(IKronosClient client)
        {
            // Arrange
            string key = Guid.NewGuid().ToString();
            byte[] data = new byte[1024];

            // Act
            await client.InsertAsync(key, data, DateTime.UtcNow.AddDays(5));
            int count = await client.CountAsync();

            // Assert
            Assert.Equal(count, 1);
        }
    }
}
