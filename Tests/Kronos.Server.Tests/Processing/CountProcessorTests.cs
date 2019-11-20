using System;
using FluentAssertions;
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
    public class CountProcessorTests
    {
        [Fact]
        public void Handle_ReturnsNumberOfElementInStorage()
        {
            // arrange
            var request = new CountRequest();
            var processor = new CountProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(Guid.NewGuid().ToString(), null, ByteString.Empty);
            storage.Add(Guid.NewGuid().ToString(), null, ByteString.Empty);

            // act
            CountResponse response = processor.Reply(request, storage);

            // assert
            response.Count.Should().Be(2);
        }

        [Fact]
        public void Handle_Returns0_WhenStorageIsEmpty()
        {
            // arrange
            var request = new CountRequest();
            var processor = new CountProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());

            // act
            CountResponse response = processor.Reply(request, storage);

            // assert
            response.Count.Should().Be(0);
        }
    }
}
