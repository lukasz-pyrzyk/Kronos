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
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handle_ReturnsTrueOrFalseIfElementIsInTheStorage(bool contains)
        {
            // arrange
            var request = new ContainsRequest();
            var processor = new ContainsProcessor();
            var storage = Substitute.For<IStorage>();
            storage.Contains(request.Key).Returns(contains);
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(contains);
            var socket = Substitute.For<ISocket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), SocketFlags.Partial)
                .Returns(expectedBytes.Length);

            // act
            processor.Handle(ref request, storage, socket);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), SocketFlags.Partial);
        }
    }
}
