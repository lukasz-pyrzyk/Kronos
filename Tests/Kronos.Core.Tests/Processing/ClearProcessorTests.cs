using Kronos.Core.Processing;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class ClearProcessorTests
    {
        [Fact]
        public void Process_ReturnsTrueOrFalseIfElementIsInTheStorage()
        {
            // arrange
            var request = new ClearRequest();
            var processor = new ClearProcessor();
            bool expected = true;
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(expected);

            // act
            byte[] response = processor.Process(request, Substitute.For<IStorage>());

            // assert
            Assert.Equal(expectedBytes, response);
        }

        [Fact]
        public void Process_ClearsTheStorage()
        {
            // arrange
            var request = new ClearRequest();
            var processor = new ClearProcessor();
            var storage = Substitute.For<IStorage>();

            // act
            processor.Process(request, storage);

            // assert
            storage.Received(1).Clear();
        }
    }
}
