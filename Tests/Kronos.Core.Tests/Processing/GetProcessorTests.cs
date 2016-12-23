using System.Linq;
using System.Net.Sockets;
using System.Text;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class GetProcessorTests
    {
        [Fact(Skip = "Awaiting System.Threading.Channels (IChannel) or TypeMock")]
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
            var socket = Substitute.For<Socket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // act
            processor.Process(ref request, storage);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
        }

        [Fact(Skip = "Awaiting System.Threading.Channels (IChannel) or TypeMock")]
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
            var socket = Substitute.For<Socket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // act
            processor.Process(ref request, storage);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
        }
    }
}
