using Kronos.Core.Messages;
using Kronos.Server.Processing;
using Kronos.Server.Storage;
using Kronos.Server.Storage.Cleaning;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Processing
{
    public class CountProcessorTests
    {
        [Fact]
        public void Handle_ReturnsNumberOfElementInStorage()
        {
            // arrange
            var request = new CountRequest();
            var processor = new CountProcessor();
            int count = 5;
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());
            storage.Count.Returns(count);

            // act
            CountResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(count, response.Count);
        }
    }
}
