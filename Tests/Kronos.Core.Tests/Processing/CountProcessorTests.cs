using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class CountProcessorTests
    {
        public void Handle_ReturnsNumberOfElementInStorage()
        {
            // arrange
            var request = new CountRequest();
            var processor = new CountProcessor();
            int expected = 5;
            var storage = Substitute.For<IStorage>();
            storage.Count.Returns(expected);

            // act
            int response = processor.Process(ref request, storage);

            // assert
            Assert.Equal(expected, response);
        }
    }
}
