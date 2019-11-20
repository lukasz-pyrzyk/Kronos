using Google.Protobuf;
using Kronos.Core.Messages;
using Kronos.Server.Processing;
using Kronos.Server.Storage;
using Kronos.Server.Storage.Cleaning;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Processing
{
    public class GetProcessorTests
    {
        [Fact]
        public void Handle_ReturnsObjectFromCache()
        {
            // arrange
            ByteString obj = ByteString.CopyFromUtf8("lorem ipsum");
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());

            storage.TryGet(request.Key, out ByteString dummy).Returns(x =>
            {
                x[1] = obj;
                return true;
            });

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            Assert.True(response.Exists);
            Assert.Equal(obj, response.Data);
        }

        [Fact]
        public void Handle_ReturnsNotFoundWhenObjectIsNotInTheCache()
        {
            // arrange
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());

            storage.TryGet(request.Key, out ByteString _).Returns(false);

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            Assert.False(response.Exists);
            Assert.True(response.Data.IsEmpty);
        }
    }
}
