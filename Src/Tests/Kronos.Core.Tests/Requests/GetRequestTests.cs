using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Xunit;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using Moq;
using NSubstitute;
using XGain.Sockets;

namespace Kronos.Core.Tests.Requests
{
    public class GetRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            GetRequest request = new GetRequest();

            Assert.Equal(request.RequestType, RequestType.Get);
        }

        [Fact]
        public void Ctor_CanAssingValues()
        {
            string key = "lorem ipsum";
            GetRequest request = new GetRequest(key);

            Assert.Equal(request.Key, key);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            GetRequest request = new GetRequest("key");

            byte[] packageBytes = SerializationUtils.Serialize(request);

            GetRequest requestFromBytes = SerializationUtils.Deserialize<GetRequest>(packageBytes);

            Assert.Equal(requestFromBytes.RequestType, request.RequestType);
            Assert.Equal(requestFromBytes.Key, request.Key);
        }

        [Fact]
        public async Task Execute_ReturnsCorrectValue()
        {
            byte[] value = SerializationUtils.Serialize("lorem ipsum");
            var request = new GetRequest("masterKey");

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(request).Returns(SerializationUtils.Serialize(value));

            byte[] response = await request.ExecuteAsync<byte[]>(communicationServiceMock);

            Assert.Equal(response, value);
            await communicationServiceMock.Received(1).SendToServerAsync(It.IsAny<GetRequest>());
        }

        [Fact]
        public async Task Execute_ReturnsNullWhenServerHasReturnedNotFound()
        {
            byte[] value = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            var request = new GetRequest("masterKey");

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(request).Returns(value);

            byte[] response = await request.ExecuteAsync<byte[]>(communicationServiceMock);

            Assert.Equal(response.Length, 1);
            Assert.Equal(response[0], 0);

            await communicationServiceMock.Received(1).SendToServerAsync(request);
        }

        [Fact]
        public void ProcessAndSendResponse_ReturnsCachedObjectToClient()
        {
            string key = "lorem ipsum";
            byte[] cachedObject = Encoding.UTF8.GetBytes("object");

            var storageMock = new Mock<IStorage>();
            storageMock.Setup(x => x.TryGet(key)).Returns(cachedObject);
            var socketMock = new Mock<ISocket>();

            var request = new GetRequest(key);
            request.ProcessAndSendResponse(socketMock.Object, storageMock.Object);

            byte[] expectedPackage = SerializationUtils.Serialize(cachedObject);
            socketMock.Verify(x => x.Send(expectedPackage), Times.Once);
        }

        [Fact]
        public void ProcessAndSendResponse_ReturnsNotFoundToClient()
        {
            string key = "lorem ipsum";
            byte[] notFoundBytes = SerializationUtils.Serialize(SerializationUtils.Serialize(RequestStatusCode.NotFound));

            var socketMock = new Mock<ISocket>();
            var storageMock = new Mock<IStorage>();
            storageMock.Setup(x => x.TryGet(key)).Returns((byte[])null);

            var request = new GetRequest(key);
            request.ProcessAndSendResponse(socketMock.Object, storageMock.Object);

            socketMock.Verify(x => x.Send(notFoundBytes), Times.Once);
        }
    }
}
