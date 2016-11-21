using System.Linq;
using System.Net.Sockets;
using System.Text;
using Kronos.Core.Networking;
using Kronos.Core.Processors;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using XGain.Sockets;
using Xunit;

namespace Kronos.Core.Tests.Processors
{
    public class GetProcessorTests
    {
        [Fact]
        public void Handle_ReturnsObjectFromCache()
        {
            // arrange
            byte[] obj = Encoding.UTF8.GetBytes("siema");
            byte[] response;
            bool expected = true;
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out response).Returns(x =>
            {
                x[1] = obj;
                return expected;
            });

            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(obj);
            var socket = Substitute.For<ISocket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // act
            processor.Handle(ref request, storage, socket);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
        }

        [Fact]
        public void Handle_ReturnsNotFoundWhenObjectIsNotInTheCache()
        {
            // arrange
            byte[] obj = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            byte[] response;
            bool expected = false;
            var request = new GetRequest();
            var processor = new GetProcessor();
            var storage = Substitute.For<IStorage>();

            storage.TryGet(request.Key, out response).Returns(x =>
            {
                x[1] = obj;
                return expected;
            });

            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(obj);
            var socket = Substitute.For<ISocket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // act
            processor.Handle(ref request, storage, socket);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
        }
    }
}
