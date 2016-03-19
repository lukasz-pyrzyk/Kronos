using System.Text;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Xunit;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using Moq;
using XGain.Sockets;

namespace Kronos.Core.Tests.Requests
{
    public class GetRequestTests
    {
        [Fact]
        public void ContainsCorrectRequestType()
        {
            GetRequest request = new GetRequest();

            Assert.Equal(request.RequestType, RequestType.GetRequest);
        }

        [Fact]
        public void CanAssignCorrectValuesByConstructor()
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
        public void Execute_ReturnsCorrectValue()
        {
            byte[] value = SerializationUtils.Serialize("lorem ipsum");
            var request = new GetRequest("masterKey");

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(SerializationUtils.Serialize(value));


            byte[] response = request.Execute(communicationServiceMock.Object);

            Assert.Equal(response, value);
            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<GetRequest>()), Times.Once);
        }

        [Fact]
        public void Execute_ReturnsNullWhenServerHasReturnedNotFound()
        {
            byte[] value = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            var request = new GetRequest("masterKey");

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(value);

            byte[] response = request.Execute(communicationServiceMock.Object);

            Assert.Null(response);
            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<GetRequest>()), Times.Once);
        }

        [Fact]
        public void ProcessRequest_ReturnsCachedObjectToClient()
        {
            string key = "lorem ipsum";
            byte[] cachedObject = Encoding.UTF8.GetBytes("object");

            var storageMock = new Mock<IStorage>();
            storageMock.Setup(x => x.TryGet(key)).Returns(cachedObject);
            var socketMock = new Mock<ISocket>();

            var request = new GetRequest(key);
            request.ProcessRequest(socketMock.Object, storageMock.Object);

            byte[] expectedPackage = SerializationUtils.Serialize(cachedObject);
            socketMock.Verify(x => x.Send(expectedPackage), Times.Once);
        }

        [Fact]
        public void ProcessRequest_ReturnsNotFoundToClient()
        {
            string key = "lorem ipsum";
            byte[] notFoundBytes = SerializationUtils.Serialize(SerializationUtils.Serialize(RequestStatusCode.NotFound));

            var storageMock = new Mock<IStorage>();
            storageMock.Setup(x => x.TryGet(key)).Returns((byte[])null);
            var socketMock = new Mock<ISocket>();

            var request = new GetRequest(key);

            request.ProcessRequest(socketMock.Object, storageMock.Object);

            socketMock.Verify(x => x.Send(notFoundBytes), Times.Once);
        }

    }
}
