using Kronos.Core.Processing;
using Kronos.Core.Serialization;
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
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(contains);

            // act
            byte[] response = processor.Process(request, storage);

            // assert
            Assert.Equal(expectedBytes, response);
        }
    }
}
