using System;
using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using NSubstitute;
using XGain.Sockets;
using Xunit;
using System.Linq;

namespace Kronos.Core.Tests.Requests
{
    public class InsertRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            InsertRequest request = new InsertRequest();

            Assert.Equal(request.RequestType, RequestType.Insert);
        }

        [Fact]
        public void Ctor_CanAssingValues()
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
        public async Task Execute_ReturnsCorrectValue(RequestStatusCode status)
        {
            var request = new InsertRequest();

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(request).Returns(SerializationUtils.Serialize(status));

            RequestStatusCode response = await request.ExecuteAsync<RequestStatusCode>(communicationServiceMock);

            Assert.Equal(response, status);
            await communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<InsertRequest>());
        }

        [Fact]
        public void ProcessAndSendResponse_AddsObjectToStorage()
        {
            string key = "lorem ipsum";
            byte[] cachedObject = SerializationUtils.Serialize("object");
            DateTime expiryDate = DateTime.Today;

            var storageMock = Substitute.For<IStorage>();
            var socketMock = Substitute.For<ISocket>();

            var request = new InsertRequest(key, cachedObject, expiryDate);
            request.ProcessAndSendResponse(socketMock, storageMock);

            storageMock.Received(1).AddOrUpdate(key, expiryDate, cachedObject);
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.Ok);
            socketMock.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(responseBytes)));
        }
    }
}
