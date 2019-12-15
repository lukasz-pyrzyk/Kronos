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
    public class InsertProcessorTests
    {
        [Fact]
        public void Handle_ReturnsTrueWhenAdded()
        {
            // arrange
            var request = new InsertRequest();
            var processor = new InsertProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());

            // Act
            InsertResponse response = processor.Reply(request, storage);

            // assert
            response.Added.Should().BeTrue();
        }

        [Fact]
        public void Handle_ReturnsFalseWhenAlreadyExists()
        {
            // arrange
            var key = "key";
            var request = new InsertRequest { Key = key };
            var processor = new InsertProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(key, null, ByteString.Empty);

            // Act
            InsertResponse response = processor.Reply(request, storage);

            // assert
            response.Added.Should().BeFalse();
        }
    }
}
