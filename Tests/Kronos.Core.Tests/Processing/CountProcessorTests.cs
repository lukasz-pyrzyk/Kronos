using Kronos.Core.Messages;
using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class CountProcessorTests
    {
        [Fact]
        public void Handle_ReturnsNumberOfElementInStorage()
        {
            // arrange
            var request = new CountRequest();
            var processor = new CountProcessor();
            int count = 5;
            var storage = Substitute.For<IStorage>();
            storage.Count.Returns(count);

            // act
            CountResponse response = processor.Reply(request, storage);

            // assert
            Assert.Equal(count, response.Count);
        }
    }
}
