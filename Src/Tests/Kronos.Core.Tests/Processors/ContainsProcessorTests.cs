using System.Linq;
using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Core.Processors;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using XGain.Sockets;
using Xunit;

namespace Kronos.Core.Tests.Processors
{
    public class ContainsProcessorTests
    {
        [Fact]
        public void Handle_ReturnsFalseWhenObjectIsNotInTheStorage()
        {
            // arrange
            bool expected = true;
            var request = new ContainsRequest();
            var processor = new ContainsProcessor();
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(false);
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock
                .Send(request)
                .Returns(SerializationUtils.SerializeToStreamWithLength(expected));
            var socket = Substitute.For<ISocket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // act
            processor.Handle(ref request, Substitute.For<IStorage>(), socket);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), SocketFlags.None);
        }

        [Fact]
        public void ProcessAndSendResponse_ReturnsCachedObjectToClient()
        {
            // arrange
            bool expected = true;
            var request = new ContainsRequest();
            var processor = new ContainsProcessor();
            var storage = Substitute.For<IStorage>();
            storage.Contains(request.Key).Returns(true);
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(true);
            var communicationServiceMock = Substitute.For<IClientServerConnection>();
            communicationServiceMock
                .Send(request)
                .Returns(SerializationUtils.SerializeToStreamWithLength(expected));
            var socket = Substitute.For<ISocket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // act
            processor.Handle(ref request, storage, socket);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), SocketFlags.None);
        }
    }
}
