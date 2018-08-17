using System.Text;
using FluentAssertions;
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
            var obj = Encoding.UTF8.GetBytes("lorem ipsum");
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out var dummy).Returns(x =>
            {
                x[1] = obj;
                return true;
            });

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            response.Should().NotBeNull();
            response.Data.ToArray().Should().Equal(obj);
        }

        [Fact]
        public void Handle_ReturnsNotFoundWhenObjectIsNotInTheCache()
        {
            // arrange
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out var _).Returns(false);

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            response.Should().NotBeNull();
            response.Data.Should().BeNull();
        }
    }
}
