using Kronos.Core.Messages;
using Kronos.Server.Processing;
using Kronos.Server.Storage;
using Kronos.Server.Storage.Cleaning;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Processing
{
    public class DeleteProcessorTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handle_ReturnsTrueOrFalseIfElementWasDeleted(bool deleted)
        {
            // arrange
            var request = new DeleteRequest();
            var processor = new DeleteProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());
            storage.TryRemove(request.Key).Returns(deleted);

            DeleteResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(response.Deleted, deleted);
        }
    }
}
