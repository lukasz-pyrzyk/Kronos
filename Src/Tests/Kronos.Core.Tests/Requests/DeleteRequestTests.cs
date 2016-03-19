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
    public class DeleteRequestTests
    {
        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            DeleteRequest request = new DeleteRequest();

            Assert.Equal(request.RequestType, RequestType.Delete);
        }

        [Fact]
        public void Ctor_CanAssingValues()
        {
            string key = "lorem ipsum";
            DeleteRequest request = new DeleteRequest(key);

            Assert.Equal(request.Key, key);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            DeleteRequest request = new DeleteRequest("key");

            byte[] packageBytes = SerializationUtils.Serialize(request);

            DeleteRequest requestFromBytes = SerializationUtils.Deserialize<DeleteRequest>(packageBytes);

            Assert.Equal(requestFromBytes.RequestType, request.RequestType);
            Assert.Equal(requestFromBytes.Key, request.Key);
        }

        [Fact]
        public void ProcessAndSendResponse_RemovesElementFromCollection()
        {
            const string key = "lorem ipsum";

            var storageMock = new Mock<IStorage>();
            storageMock.Setup(x => x.TryRemove(key)).Returns(true);
            var socketMock = new Mock<ISocket>();

            var request = new DeleteRequest(key);
            request.ProcessAndSendResponse(socketMock.Object, storageMock.Object);

            storageMock.Verify(x => x.TryRemove(key), Times.Once);
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.Deleted);
            socketMock.Verify(x => x.Send(responseBytes), Times.Once);
        }

        [Fact]
        public void ProcessAndSendResponse_DoesNotRemoveElementFromCollectionWhenNotExist()
        {
            const string key = "lorem ipsum";

            var storageMock = new Mock<IStorage>();
            storageMock.Setup(x => x.TryRemove(key)).Returns(false);
            var socketMock = new Mock<ISocket>();

            var request = new DeleteRequest(key);
            request.ProcessAndSendResponse(socketMock.Object, storageMock.Object);

            storageMock.Verify(x => x.TryRemove(key), Times.Once);
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            socketMock.Verify(x => x.Send(responseBytes), Times.Once);
        }
    }
}
