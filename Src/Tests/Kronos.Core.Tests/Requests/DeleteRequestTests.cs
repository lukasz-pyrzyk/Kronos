using Kronos.Core.Requests;
using Xunit;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using NSubstitute;
using XGain.Sockets;
using System.Linq;

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

            var storageMock = Substitute.For<IStorage>();
            storageMock.TryRemove(key).Returns(true);
            var socketMock = Substitute.For<ISocket>();

            var request = new DeleteRequest(key);
            request.ProcessAndSendResponse(socketMock, storageMock);

            storageMock.Received(1).TryRemove(key);
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.Deleted);
            socketMock.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(responseBytes)));
        }

        [Fact]
        public void ProcessAndSendResponse_DoesNotRemoveElementFromCollectionWhenNotExist()
        {
            const string key = "lorem ipsum";

            var storageMock = Substitute.For<IStorage>();
            storageMock.TryRemove(key).Returns(false);
            var socketMock = Substitute.For<ISocket>();

            var request = new DeleteRequest(key);
            request.ProcessAndSendResponse(socketMock, storageMock);

            storageMock.Received(1).TryRemove(key);
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            socketMock.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(responseBytes)));
        }
    }
}
