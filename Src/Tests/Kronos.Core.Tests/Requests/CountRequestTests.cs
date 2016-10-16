using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using Moq;
using NSubstitute;
using XGain.Sockets;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class CountRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            var request = new CountRequest();

            Assert.Equal(request.RequestType, RequestType.Count);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            var request = new CountRequest();

            byte[] packageBytes = SerializationUtils.Serialize(request);

            CountRequest requestFromBytes = SerializationUtils.Deserialize<CountRequest>(packageBytes);

            Assert.NotNull(requestFromBytes);
        }

        [Fact]
        public async Task Execute_ReturnsCorrectValue()
        {
            int value = 5;
            var request = new CountRequest();

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(request).Returns(SerializationUtils.Serialize(value));

            int response = await request.ExecuteAsync<int>(communicationServiceMock);

            Assert.Equal(response, value);
            await communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<CountRequest>());
        }

        [Fact]
        public void ProcessAndSendResponse_ReturnsCachedObjectToClient()
        {
            int expectecCount = 5;

            var storageMock = Substitute.For<IStorage>();
            storageMock.Count.Returns(expectecCount);
            var socketMock = new Mock<ISocket>();

            var request = new CountRequest();
            request.ProcessAndSendResponse(socketMock.Object, storageMock);

            byte[] expectedPackage = SerializationUtils.Serialize(expectecCount);
            socketMock.Verify(x => x.Send(expectedPackage), Times.Once);
        }
    }
}
