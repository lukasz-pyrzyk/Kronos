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
    public class InsertProcessorTests
    {
        [Theory]
        [InlineData(true)]
        public void Handle_ReturnsTrueWhenElementAdded(bool added)
        {
            // arrange
            bool expected = true;
            var request = new InsertRequest();
            var processor = new InsertProcessor();
            var storage = Substitute.For<IStorage>();
            storage.TryRemove(request.Key).Returns(added);
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(added);
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
