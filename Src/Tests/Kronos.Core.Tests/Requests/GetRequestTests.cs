using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Xunit;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
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

            Assert.Equal(request.Type, RequestType.Get);
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

            Assert.Equal(requestFromBytes.Type, request.Type);
            Assert.Equal(requestFromBytes.Key, request.Key);
        }

        //[Fact]
        //public async Task Execute_ReturnsCorrectValue()
        //{
        //    byte[] value = SerializationUtils.Serialize("lorem ipsum");
        //    var request = new GetRequest("masterKey");

        //    var communicationServiceMock = Substitute.For<IClientServerConnection>();
        //    communicationServiceMock.Send(request).Returns(SerializationUtils.SerializeToStreamWithLength(value));

        //    byte[] response = await request.ExecuteAsync<byte[]>(communicationServiceMock);

        //    Assert.Equal(response, value);
        //    await communicationServiceMock.Received(1).Send(Arg.Any<GetRequest>());
        //}

        //[Fact]
        //public async Task Execute_ReturnsNullWhenServerHasReturnedNotFound()
        //{
        //    byte[] value = SerializationUtils.Serialize(RequestStatusCode.NotFound);
        //    var request = new GetRequest("masterKey");

        //    var communicationServiceMock = Substitute.For<IClientServerConnection>();
        //    communicationServiceMock.Send(request).Returns(value);

        //    byte[] response = await request.ExecuteAsync<byte[]>(communicationServiceMock);

        //    Assert.Equal(response.Length, 1);
        //    Assert.Equal(response[0], 0);

        //    await communicationServiceMock.Received(1).Send(request);
        //}

        //[Fact]
        //public void ProcessAndSendResponse_ReturnsCachedObjectToClient()
        //{
        //    string key = "lorem ipsum";
        //    byte[] cachedObject = Encoding.UTF8.GetBytes("object");

        //    var storageMock = Substitute.For<IStorage>();
        //    storageMock.TryGet(key).Returns(cachedObject);
        //    var socketMock = Substitute.For<ISocket>();

        //    var request = new GetRequest(key);
        //    request.ProcessAndSendResponse(socketMock, storageMock);

        //    byte[] expectedPackage = SerializationUtils.SerializeToStreamWithLength(cachedObject);
        //    socketMock.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedPackage)));
        //}

        //[Fact]
        //public void ProcessAndSendResponse_ReturnsNotFoundToClient()
        //{
        //    string key = "lorem ipsum";
        //    byte[] notFoundBytes = SerializationUtils.SerializeToStreamWithLength(SerializationUtils.Serialize(RequestStatusCode.NotFound));

        //    var socketMock = Substitute.For<ISocket>();
        //    var storageMock = Substitute.For<IStorage>();
        //    storageMock.TryGet(key).Returns((byte[])null);

        //    var request = new GetRequest(key);
        //    request.ProcessAndSendResponse(socketMock, storageMock);

        //    socketMock.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(notFoundBytes)));
        //}
    }
}
