using Kronos.Core.Messages;
using Kronos.Server.Processing;
using Kronos.Server.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Processing
{
    public class ContainsProcessorTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handle_ReturnsTrueOrFalseIfElementIsInTheStorage(bool contains)
        {
            // arrange
            var request = new ContainsRequest();
            var processor = new ContainsProcessor();
            var storage = Substitute.For<IStorage>();
            storage.Contains(request.Key).Returns(contains);

            // act
            ContainsResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(response.Contains, contains);
        }
    }
}
