using Google.Protobuf;
using Kronos.Core.Messages;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
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
            var storage = Substitute.For<IStorage>();

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
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out ByteString _).Returns(false);

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            Assert.False(response.Exists);
            Assert.True(response.Data.IsEmpty);
        }
    }
}
