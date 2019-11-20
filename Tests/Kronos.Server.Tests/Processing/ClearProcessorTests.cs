using Kronos.Core.Messages;
using Kronos.Server.Processing;
using Kronos.Server.Storage;
using Kronos.Server.Storage.Cleaning;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Processing
{
    public class ClearProcessorTests
    {
        [Fact]
        public void Process_ReturnsNumberOfDeleted()
        {
            // arrange
            var request = new ClearRequest();
            var processor = new ClearProcessor();
            var deletedCount = 5;
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());
            storage.Clear().Returns(deletedCount);

            // act
            ClearResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(response.Deleted, deletedCount);
        }

        [Fact]
        public void Process_ClearsTheStorage()
        {
            // arrange
            var request = new ClearRequest();
            var processor = new ClearProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());

            // act
            processor.Reply(request, storage);

            // assert
            storage.Received(1).Clear();
        }
    }
}
