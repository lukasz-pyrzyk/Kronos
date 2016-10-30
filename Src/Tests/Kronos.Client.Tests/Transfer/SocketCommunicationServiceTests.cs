using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Kronos.Client.Transfer;
using Kronos.Core.Communication;
using Kronos.Core.Exceptions;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using XGain.Sockets;
using Xunit;

namespace Kronos.Client.Tests.Transfer
{
    public class SocketCommunicationServiceTests
    {
        [Fact]
        public async Task SendToServer_WorksCorrect()
        {
            // arrange
            var request = new InsertRequest();
            var socket = PrepareSocket(request);

            var ipEndpoint = new IPEndPoint(IPAddress.Any, 500);

            var service = new SocketCommunicationService(ipEndpoint, () => socket);

            // act
            await service.SendToServerAsync(request);

            // assert
            socket.Received(1).Connect(ipEndpoint);
            socket.Received(2).Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>());
            socket.Received(1).Dispose();
        }

        [Fact]
        public async Task SendToServer_Dispose_WasCatched_SocketException()
        {
            // arrange
            var request = new InsertRequest();
            var socket = PrepareSocket(request);

            socket.When(x => x.Dispose()).Do(x => { throw new SocketException(); });

            var ipEndpoint = new IPEndPoint(IPAddress.Any, 500);

            var service = new SocketCommunicationService(ipEndpoint, () => socket);

            //  act and assert
            await service.SendToServerAsync(request);
        }

        [Fact]
        public async Task SendToServer_Dispose_WasNowCatched_ArgumentNullException()
        {
            // arrange
            var request = new InsertRequest();
            var socket = PrepareSocket(request);

            socket.When(x => x.Dispose()).Do(x => { throw new ArgumentNullException(); });

            var ipEndpoint = new IPEndPoint(IPAddress.Any, 500);

            var service = new SocketCommunicationService(ipEndpoint, () => socket);

            //  act and assert
            await Assert.ThrowsAsync(typeof(ArgumentNullException), () => service.SendToServerAsync(request));
        }

        private static ISocket PrepareSocket(Request request)
        {
            var data = PrepareData(request);
            var socket = Substitute.For<ISocket>();
            socket.BufferSize.Returns(4 * 1024);
            socket.Send(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SocketFlags>())
                .Returns(data.Length);

            return socket;
        }

        private static byte[] PrepareData(Request request)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationUtils.SerializeToStream(ms, request.RequestType);
                SerializationUtils.SerializeToStream(ms, request);
                return ms.ToArray();
            }
        }
    }
}
