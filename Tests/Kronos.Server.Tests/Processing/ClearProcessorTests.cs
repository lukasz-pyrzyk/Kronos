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
    public class ClearProcessorTests
    {
        [Fact]
        public void Process_ReturnsNumberOfDeletedAndClearsTheData()
        {
            // arrange
            var request = new ClearRequest();
            var processor = new ClearProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add("name", DateTimeOffset.MaxValue, ByteString.Empty);

            // act
            ClearResponse response = processor.Reply(request, storage);

            // assert
            response.Deleted.Should().Be(1);
            storage.Count.Should().Be(0);
        }
    }
}
