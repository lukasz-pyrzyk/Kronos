using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
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
            var server = new ServerConfig();
            IConnection con = Substitute.For<IConnection>();
            con.SendAsync(request, server).Returns(fakeData);
            var processor = new FakeProcessor();

            // Act
            bool result = await processor.ExecuteAsync(request, con, server);

            // Assert
            await con.Received(1).SendAsync(request, server);
            Assert.Equal(fakeResult, result);
        }

        [Fact]
        public void SendsDataToTheClient()
        {
            // Arrange
            var request = new InsertRequest();
            var processor = new FakeProcessor();
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(true);

            // Act
            byte[] response = processor.Process(request, Substitute.For<IStorage>());

            // assert
            Assert.Equal(expectedBytes, response);
        }

        internal class FakeProcessor : CommandProcessor<InsertRequest, bool>
        {
            public override byte[] Process(InsertRequest request, IStorage storage)
            {
                return Reply(true);
            }
        }
    }
}
