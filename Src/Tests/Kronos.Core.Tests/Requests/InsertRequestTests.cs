using System;
using System.Text;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using Moq;
using XGain.Sockets;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class InsertRequestTests
    {
        [Fact]
        public void ContainsCorrectRequestType()
        {
            InsertRequest request = new InsertRequest();

            Assert.Equal(request.RequestType, RequestType.InsertRequest);
        }

        [Fact]
        public void CanAssingPropertiesByConstructor()
        {
            string key = "key";
            byte[] serializedObject = Encoding.UTF8.GetBytes("lorem ipsum");
            DateTime expiryDate = DateTime.Today;

            InsertRequest request = new InsertRequest(key, serializedObject, expiryDate);

            Assert.NotNull(request);
            Assert.Equal(request.Key, key);
            Assert.Equal(request.Object, serializedObject);
            Assert.Equal(request.ExpiryDate, expiryDate);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            InsertRequest request = new InsertRequest
            {
                Object = Encoding.UTF8.GetBytes("lorem ipsum"),
                ExpiryDate = DateTime.Now,
                Key = "key"
            };

            byte[] packageBytes = SerializationUtils.Serialize(request);

            InsertRequest requestFromBytes = SerializationUtils.Deserialize<InsertRequest>(packageBytes);

            Assert.Equal(requestFromBytes.Object, request.Object);
            Assert.Equal(requestFromBytes.ExpiryDate, request.ExpiryDate);
            Assert.Equal(requestFromBytes.Key, request.Key);
        }

        [Theory]
        [InlineData(RequestStatusCode.Ok)]
        [InlineData(RequestStatusCode.Failed)]
        public void ExecuteReturnsCorrectValue(RequestStatusCode status)
        {
            var request = new InsertRequest();

            var communicationServiceMock = new Mock<IClientServerConnection>();
            communicationServiceMock.Setup(x => x.SendToServer(request)).Returns(SerializationUtils.Serialize(status));

            RequestStatusCode response = request.ProcessRequest<RequestStatusCode>(communicationServiceMock.Object);

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

            var request = new InsertRequest(key, cachedObject, expiryDate);
            request.ProcessResponse(socketMock.Object, storageMock.Object);

            storageMock.Verify(x => x.AddOrUpdate(key, cachedObject), Times.Once);
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.Ok);
            socketMock.Verify(x => x.Send(responseBytes), Times.Once);
        }
    }
}
