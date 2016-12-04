using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class InsertProcessorTests
    {
        [Theory(Skip = "Awaiting System.Threading.Channels (IChannel) or TypeMock")]
        [InlineData(true)]
        public async Task Handle_ReturnsTrueWhenElementAdded(bool added)
        {
            // arrange
            var request = new InsertRequest();
            var processor = new InsertProcessor();
            var storage = Substitute.For<IStorage>();
            storage.TryRemove(request.Key).Returns(added);
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(added);
            var socket = Substitute.For<Socket>();
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // act
            await processor.HandleAsync(request, storage, socket);

            // assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
        }
    }
}
