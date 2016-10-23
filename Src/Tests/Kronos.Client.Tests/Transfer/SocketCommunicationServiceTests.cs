using System;
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
using Moq;
using NSubstitute;
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
            var socket = Substitute.For<ISocket>();
            var request = new InsertRequest();
            var ipEndpoint = new IPEndPoint(IPAddress.Any, 500);

            var service = new SocketCommunicationService(ipEndpoint, () => socket);

            // act
            await service.SendToServerAsync(request);

            // assert
            socket.Received(1).Connect(ipEndpoint);
            socket.Received(2).Send(Arg.Any<byte[]>());
            socket.Received(1).Dispose();
        }

        [Fact]
        public async Task SendToServer_CatchsExceptionFromSocketDispose()
        {
            // arrange
            var socket = Substitute.For<ISocket>();
            socket.Dispose();

            var request = new InsertRequest();
            var ipEndpoint = new IPEndPoint(IPAddress.Any, 500);

            var service = new SocketCommunicationService(ipEndpoint, () => socket);

            //  act
            await service.SendToServerAsync(request);

            socket.Received(1).Connect(ipEndpoint);
            socket.Received(2).Send(Arg.Any<byte[]>());
            socket.Received(1).Dispose();
        }
    }
}
