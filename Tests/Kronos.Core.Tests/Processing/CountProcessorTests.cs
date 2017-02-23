using Kronos.Core.Processing;
using Kronos.Core.Serialization;
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
            int count = 5;
            var storage = Substitute.For<IStorage>();
            storage.Count.Returns(count);

            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(count);

            // act
            byte[] response = processor.Process(request, storage);

            // assert
            Assert.Equal(expectedBytes, response);
        }
    }
}
