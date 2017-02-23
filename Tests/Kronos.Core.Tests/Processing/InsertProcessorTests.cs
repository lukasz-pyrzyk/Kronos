using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class InsertProcessorTests
    {
        [InlineData(true)]
        public void Handle_ReturnsTrueWhenElementAdded(bool added)
        {
            // arrange
            var request = new InsertRequest();
            var processor = new InsertProcessor();
            var storage = Substitute.For<IStorage>();
            storage.TryRemove(request.Key).Returns(added);
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(added);

            // Act
            byte[] response = processor.Process(request, storage);

            // assert
            Assert.Equal(expectedBytes, response);
        }
    }
}
