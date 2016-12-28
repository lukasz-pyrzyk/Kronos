using Kronos.Core.Processing;
using Kronos.Core.Requests;
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
            // Act
            bool response = processor.Process(ref request, storage);

            // assert
            Assert.Equal(added, response);
        }
    }
}
