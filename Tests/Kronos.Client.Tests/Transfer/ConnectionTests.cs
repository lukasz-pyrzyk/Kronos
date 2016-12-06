using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Client.Transfer;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using NSubstitute;
using Xunit;

namespace Kronos.Client.Tests.Transfer
{
    public class ConnectionTests
    {
        [Fact(Skip = "Awaiting System.Threading.Channels (IChannel) or TypeMock")]
        public async Task SendToServer_WorksCorrect()
        {
            // arrange
            var request = new InsertRequest();
            var socket = PrepareSocket(request);

            var ipEndpoint = new IPEndPoint(IPAddress.Any, 500);

            var service = new Connection(ipEndpoint);

            // act
            await service.SendAsync(request);

            // assert
            socket.Received(1).Connect(ipEndpoint);
            socket.Received(2).Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
            socket.Received(1).Dispose();
        }

        [Fact(Skip = "Awaiting System.Threading.Channels (IChannel) or TypeMock")]
        public async Task SendToServer_Dispose_WasNowCatched_ArgumentNullException()
        {
            // arrange
            var request =new InsertRequest();
            var socket = PrepareSocket(request);

            socket.When(x => x.Dispose()).Do(x => { throw new ArgumentNullException(); });

            var ipEndpoint = new IPEndPoint(IPAddress.Any, 500);

            var service = new Connection(ipEndpoint, 0);

            //  act and assert
            await Assert.ThrowsAsync(typeof(ArgumentNullException), async () => await service.SendAsync(request));
        }

        private static Socket PrepareSocket(IRequest request)
        {
            var data = PrepareData(request);
            var socket = Substitute.For<Socket>();
            // socket.BufferSize.Returns(4 * 1024);
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(4, data.Length);
            socket.Receive(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(4, data.Length);

            return socket;
        }

        private static byte[] PrepareData(IRequest request)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationUtils.SerializeToStream(ms, request.Type);
                SerializationUtils.SerializeToStream(ms, request);
                return ms.ToArray();
            }
        }
    }
}
