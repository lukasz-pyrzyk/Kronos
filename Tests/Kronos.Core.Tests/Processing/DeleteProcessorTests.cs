using Kronos.Core.Processing;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class DeleteProcessorTests
    {
        [InlineData(true)]
        [InlineData(false)]
        public void Handle_ReturnsTrueOrFalseIfElementWasDeleted(bool deleted)
        {
            // arrange
            var request = new DeleteRequest();
            var processor = new DeleteProcessor();
            var storage = Substitute.For<IStorage>();
            storage.TryRemove(request.Key).Returns(deleted);
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(deleted);

            byte[] response = processor.Process(request, storage);

            // assert
            Assert.Equal(expectedBytes, response);
        }
    }
}
