using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using XGain.Sockets;
using Xunit;
using System.Linq;

namespace Kronos.Core.Tests.Requests
{
    public class ContainsRequestTests
    {

        [Fact]
        public void Ctor_AssignsProperties()
        {
            string key = "lorem ipsum";
            var request = new ContainsRequest(key);

            Assert.Equal(key, request.Key);
        }

        [Fact]
        public void RequestType_ContainsCorrectType()
        {
            var request = new ContainsRequest();

            Assert.Equal(request.RequestType, RequestType.Contains);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
        {
            string key = "lorem ipsum";
            var request = new ContainsRequest(key);

            byte[] packageBytes = SerializationUtils.Serialize(request);

            ContainsRequest requestFromBytes = SerializationUtils.Deserialize<ContainsRequest>(packageBytes);

            Assert.NotNull(requestFromBytes);
            Assert.Equal(requestFromBytes.Key, key);
        }

        [Fact]
        public async Task Execute_ReturnsCorrectValue()
        {
            bool expected = true;
            var request = new ContainsRequest();

            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock.SendToServerAsync(request).Returns(SerializationUtils.Serialize(expected));

            bool response = await request.ExecuteAsync<bool>(communicationServiceMock);

            Assert.Equal(response, expected);
            await communicationServiceMock.Received(1).SendToServerAsync(Arg.Any<ContainsRequest>());
        }

        [Fact]
        public void ProcessAndSendResponse_ReturnsCachedObjectToClient()
        {
            bool expected = true;
            string key = "lorem ipsum";

            var storageMock = Substitute.For<IStorage>();
            storageMock.Contains(key).Returns(expected);
            var socketMock = Substitute.For<ISocket>();

            var request = new ContainsRequest(key);
            request.ProcessAndSendResponse(socketMock, storageMock);

            byte[] expectedPackage = SerializationUtils.Serialize(expected);
            bool result = SerializationUtils.Deserialize<bool>(expectedPackage);
            Assert.Equal(expected, result);
            socketMock.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedPackage)));
        }
    }
}
