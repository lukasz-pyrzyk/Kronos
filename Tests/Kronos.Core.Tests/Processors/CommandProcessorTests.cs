using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
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
    public class CommandProcessorTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsService()
        {
            // Arrange
            bool fakeResult = true;
            byte[] fakeData = SerializationUtils.SerializeToStreamWithLength(fakeResult);
            var request = new InsertRequest();
            IClientServerConnection connection = Substitute.For<IClientServerConnection>();
            connection.Send(request).Returns(fakeData);
            var processor = new FakeProcessor();

            // Act
            bool result = await processor.ExecuteAsync(request, connection);

            // Assert
            connection.Received(1).Send(request);
            Assert.Equal(fakeResult, result);
        }

        [Fact]
        public void SendsDataToTheClient()
        {
            // Arrange
            var request = new InsertRequest();
            var processor = new FakeProcessor();
            var socket = Substitute.For<ISocket>();
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(true);

            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), SocketFlags.Partial)
                .Returns(expectedBytes.Length);

            // Act
            processor.Handle(ref request, Substitute.For<IStorage>(), socket);

            // Assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), SocketFlags.Partial);
        }

        internal class FakeProcessor : CommandProcessor<InsertRequest, bool>
        {
            public override RequestType Type { get; }
            public override void Handle(ref InsertRequest request, IStorage storage, ISocket client)
            {
                Reply(true, client);
            }
        }
    }
}
