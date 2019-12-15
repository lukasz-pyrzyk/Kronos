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
    public class ContainsProcessorTests
    {
        [Fact]
        public void Handle_ReturnsTrueWhenElementExists()
        {
            // arrange
            var key = "key";
            var request = new ContainsRequest { Key = key };
            var processor = new ContainsProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(key, DateTimeOffset.MaxValue, ByteString.Empty);

            // act
            ContainsResponse response = processor.Reply(request, storage);

            // assert
            response.Contains.Should().BeTrue();
        }

        [Fact]
        public void Handle_ReturnsFalseWhenElementDoesntExist()
        {
            // arrange
            var request = new ContainsRequest();
            var processor = new ContainsProcessor();
            var storage = new InMemoryStorage(Substitute.For<ILogger<InMemoryStorage>>());

            // act
            ContainsResponse response = processor.Reply(request, storage);

            // assert
            response.Contains.Should().BeFalse();
        }
    }
}
