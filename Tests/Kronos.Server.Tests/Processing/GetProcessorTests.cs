using System;
using FluentAssertions;
using Google.Protobuf;
using Kronos.Core.Messages;
using Kronos.Server.Processing;
using Kronos.Server.Storage;
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
            var key = "key";
            ByteString obj = ByteString.CopyFromUtf8("lorem ipsum");
            var request = new GetRequest { Key = key };
            var processor = new GetProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(key, DateTimeOffset.MaxValue, obj);

            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            response.Exists.Should().BeTrue();
            response.Data.Should().BeEquivalentTo(obj);
        }

        [Fact]
        public void Handle_ReturnsNotFoundWhenObjectIsNotInTheCache()
        {
            // arrange
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());
            
            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            response.Exists.Should().BeFalse();
            response.Data.Should().BeEmpty();
        }

        [Fact]
        public void Handle_ReturnsNotFoundWhenKeyDoesntMatch()
        {
            // arrange
            var key = "key";
            ByteString obj = ByteString.CopyFromUtf8("lorem ipsum");
            var request = new GetRequest { Key = Guid.NewGuid().ToString() };
            var processor = new GetProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(key, DateTimeOffset.MaxValue, obj);
            
            // Act
            GetResponse response = processor.Reply(request, storage);

            // assert
            response.Exists.Should().BeFalse();
            response.Data.Should().BeEmpty();
        }
    }
}
