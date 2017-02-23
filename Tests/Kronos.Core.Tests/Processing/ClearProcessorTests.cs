using Kronos.Core.Messages;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
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
            var storage = Substitute.For<IStorage>();
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
            var storage = Substitute.For<IStorage>();

            // act
            processor.Reply(request, storage);

            // assert
            storage.Received(1).Clear();
        }
    }
}
