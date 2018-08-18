﻿using System;
using Google.Protobuf;
using Kronos.Core.Messages;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
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
            var storage = Substitute.For<IStorage>();
            storage.Add(Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<ReadOnlyMemory<byte>>()).Returns(added);

            // Act
            InsertResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(response.Added, added);
        }
    }
}
