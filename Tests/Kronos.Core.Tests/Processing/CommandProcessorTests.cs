﻿using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Networking;
using Kronos.Core.Processing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using NSubstitute;
using Xunit;

namespace Kronos.Core.Tests.Processing
{
    public class CommandProcessorTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsService()
        {
            // Arrange
            bool fakeResult = true;
            byte[] fakeData = SerializationUtils.Serialize(fakeResult);
            var request = new InsertRequest();
            IConnection connection = Substitute.For<IConnection>();
            connection.SendAsync(request).Returns(fakeData);
            var processor = new FakeProcessor();

            // Act
            bool result = await processor.ExecuteAsync(request, connection);

            // Assert
            await connection.Received(1).SendAsync(request);
            Assert.Equal(fakeResult, result);
        }

        [Fact(Skip = "Awaiting System.Threading.Channels (IChannel) or TypeMock")]
        public async Task SendsDataToTheClient()
        {
            // Arrange
            var request = new InsertRequest();
            var processor = new FakeProcessor();
            var socket = Substitute.For<Socket>();
            byte[] expectedBytes = SerializationUtils.SerializeToStreamWithLength(true);

            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(expectedBytes.Length);

            // Act
            await processor.HandleAsync(request, Substitute.For<IStorage>(), socket);

            // Assert
            socket.Received(1).Send(Arg.Is<byte[]>(x => x.SequenceEqual(expectedBytes)), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
        }

        internal class FakeProcessor : CommandProcessor<InsertRequest, bool>
        {
            public override RequestType Type { get; }

            public override async Task HandleAsync(InsertRequest request, IStorage storage, Socket client)
            {
                await ReplyAsync(true, client);
            }
        }
    }
}
