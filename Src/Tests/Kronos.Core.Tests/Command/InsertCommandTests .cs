using System;
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
    public class InsertCommandTests
    {
        [Theory]
        [InlineData(RequestStatusCode.Ok)]
        [InlineData(RequestStatusCode.Failed)]
        public void ExecuteReturnsCorrectValue(RequestStatusCode status)
        {
            var request = new InsertRequest();

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(SerializationUtils.Serialize(status));

            InsertCommand command = new InsertCommand();
            RequestStatusCode response = command.Execute(communicationServiceMock.Object, request);

            Assert.Equal(response, status);
            communicationServiceMock.Verify(x => x.SendToServer(It.IsAny<InsertRequest>()), Times.Exactly(1));
        }

        [Fact]
        public void ProcessRequest_AddsObjectToStorage()
        {
            string key = "lorem ipsum";
            byte[] cachedObject = SerializationUtils.Serialize("object");
            DateTime expiryDate = DateTime.Today;

            var storageMock = new Mock<IStorage>();
            var socketMock = new Mock<ISocket>();

            InsertCommand command = new InsertCommand();
            byte[] requtestBytes = SerializationUtils.Serialize(new InsertRequest(key, cachedObject, expiryDate));
            command.ProcessRequest(socketMock.Object, requtestBytes, storageMock.Object);

            storageMock.Verify(x => x.AddOrUpdate(key, cachedObject), Times.Once);
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.Ok);
            socketMock.Verify(x => x.Send(responseBytes), Times.Once);
        }
    }
}
