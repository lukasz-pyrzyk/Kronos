using System.Text;
using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using Moq;
using XGain.Sockets;
using Xunit;

namespace Kronos.Core.Tests.Command
{
    public class GetCommandTests
    {
        [Fact]
        public void Execute_ReturnsCorrectValue()
        {
            byte[] value = SerializationUtils.Serialize("lorem ipsum");
            var request = new GetRequest("masterKey");

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(value);

            GetCommand command = new GetCommand();

            byte[] response = command.Execute(communicationServiceMock.Object, request);

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

            GetCommand command = new GetCommand();

            byte[] response = command.Execute(communicationServiceMock.Object, request);

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

            GetCommand command = new GetCommand();
            byte[] requtestBytes = SerializationUtils.Serialize(new GetRequest(key));
            command.ProcessRequest(socketMock.Object, requtestBytes, storageMock.Object);

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

            GetCommand command = new GetCommand();
            byte[] requtestBytes = SerializationUtils.Serialize(new GetRequest(key));

            command.ProcessRequest(socketMock.Object, requtestBytes, storageMock.Object);

            socketMock.Verify(x => x.Send(notFoundBytes), Times.Once);
        }
    }
}
