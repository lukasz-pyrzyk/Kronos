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
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Handle_ReturnsObjectFromCache(bool expected)
        {
            // arrange
            ByteString obj = ByteString.CopyFromUtf8("lorem ipsum");
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out ByteString dummy).Returns(x =>
            {
                x[1] = obj;
                return expected;
            });

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(expected, response.Exists);
            Assert.Equal(obj, response.Data);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Handle_ReturnsNotFoundWhenObjectIsNotInTheCache(bool expected)
        {
            // arrange
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out ByteString _).Returns(expected);

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(expected, response.Exists);
            Assert.True(response.Data.IsEmpty);
        }
    }
}
