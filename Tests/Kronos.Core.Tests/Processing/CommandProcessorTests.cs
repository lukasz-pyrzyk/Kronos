using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
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
            var fakeResult = new Response { InsertResponse = new InsertResponse { Added = true } };
            var request = new Request { InsertRequest = new InsertRequest(), Type = RequestType.Insert };
            var server = new ServerConfig();
            IConnection con = Substitute.For<IConnection>();
            con.SendAsync(request, server).Returns(fakeResult);
            var processor = new FakeProcessor();

            // Act
            InsertResponse response = await processor.ExecuteAsync(request, con, server);

            // Assert
            await con.Received(1).SendAsync(request, server);
            Assert.Equal(fakeResult.InsertResponse, response);
        }

        [Fact]
        public void SendsDataToTheClient()
        {
            // Arrange
            var request = new InsertRequest();
            var processor = new FakeProcessor();
            // Act
            var response = processor.Reply(request, Substitute.For<IStorage>());

            // assert
            Assert.NotNull(response);
        }

        internal class FakeProcessor : CommandProcessor<InsertRequest, InsertResponse>
        {
            public override InsertResponse Reply(InsertRequest request, IStorage storage)
            {
                return new InsertResponse();
            }

            protected override InsertResponse ParseResponse(Response response)
            {
                return response.InsertResponse;
            }
        }
    }
}
