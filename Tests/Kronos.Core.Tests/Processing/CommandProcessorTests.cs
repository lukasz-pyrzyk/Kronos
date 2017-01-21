using System.Threading.Tasks;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class CommandProcessorTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsService()
        {
            // Arrange
            bool fakeResult = true;
            byte[] fakeData = SerializationUtils.Serialize(fakeResult);
            var request = new InsertRequest();
            IConnection connection = Substitute.For<IConnection>();
            connection.SendAsync(request).Returns(fakeData);
            var processor = new FakeProcessor();

            // Act
            bool result = await processor.ExecuteAsync(request, connection);

            // Assert
            await connection.Received(1).SendAsync(request);
            Assert.Equal(fakeResult, result);
        }

        [Fact]
        public void SendsDataToTheClient()
        {
            // Arrange
            var request = new InsertRequest();
            var processor = new FakeProcessor();
            bool expected = true;

            // Act
            bool response = processor.Process(ref request, Substitute.For<IStorage>());

            // assert
            Assert.Equal(expected, response);
        }

        internal class FakeProcessor : CommandProcessor<InsertRequest, bool>
        {
            public override RequestType Type { get; }

            public override bool Process(ref InsertRequest request, IStorage storage)
            {
                return true;
            }
        }
    }
}
