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
    public class DeleteProcessorTests
    {
        [Fact]
        public void Handle_ReturnsTrueWhenElementWasDeleted()
        {
            // arrange
            var key = "key";
            var request = new DeleteRequest { Key = key };
            var processor = new DeleteProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(key, null, ByteString.Empty);

            DeleteResponse response = processor.Reply(request, storage);

            // assert
            response.Deleted.Should().BeTrue();
            storage.Count.Should().Be(0);
        }

        [Fact]
        public void Handle_ReturnsFalseWhenElementWasNotAdded()
        {
            // arrange
            var key = "key";
            var request = new DeleteRequest { Key = key };
            var processor = new DeleteProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());

            DeleteResponse response = processor.Reply(request, storage);

            // assert
            response.Deleted.Should().BeFalse();
        }

        [Fact]
        public void Handle_ReturnsFalseWhenKeyWasDifferent()
        {
            // arrange
            var key = "key";
            var request = new DeleteRequest { Key = Guid.NewGuid().ToString() };
            var processor = new DeleteProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(key, null, ByteString.Empty);

            DeleteResponse response = processor.Reply(request, storage);

            // assert
            response.Deleted.Should().BeFalse();
            storage.Count.Should().Be(1);
        }
    }
}
