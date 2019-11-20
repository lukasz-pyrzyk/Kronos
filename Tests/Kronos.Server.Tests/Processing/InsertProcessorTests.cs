using System;
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
    public class InsertProcessorTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handle_ReturnsTrueWhenElementAdded(bool added)
        {
            // arrange
            var request = new InsertRequest();
            var processor = new InsertProcessor();
            var storage = new InMemoryStorage(Substitute.For<ICleaner>(), Substitute.For<IScheduler>(), Substitute.For<ILogger<InMemoryStorage>>());
            storage.Add(Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<ByteString>()).Returns(added);

            // Act
            InsertResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(response.Added, added);
        }
    }
}
