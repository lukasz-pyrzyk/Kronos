﻿using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class ContainsProcessorTests
    {
        [InlineData(true)]
        [InlineData(false)]
        public void Handle_ReturnsTrueOrFalseIfElementIsInTheStorage(bool contains)
        {
            // arrange
            var request = new ContainsRequest();
            var processor = new ContainsProcessor();
            var storage = Substitute.For<IStorage>();
            storage.Contains(request.Key).Returns(contains);
            bool expected = contains;

            // act
            bool response = processor.Process(ref request, storage);

            // assert
            Assert.Equal(expected, response);
        }
    }
}
