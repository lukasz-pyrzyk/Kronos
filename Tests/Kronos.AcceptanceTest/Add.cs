using System;
using System.Threading.Tasks;
using Kronos.Client;
using Xunit;

namespace Kronos.AcceptanceTest
{
    public class Add : Base
    {
        protected override async Task ProcessAsync(IKronosClient client)
        {
            // Arrange
            string key = Guid.NewGuid().ToString();
            byte[] data = new byte[1024];

            // Act
            await client.InsertAsync(key, data, DateTime.UtcNow.AddDays(5));
            bool contains = await client.ContainsAsync(key);

            // Assert
            Assert.True(contains);
        }
    }
}
