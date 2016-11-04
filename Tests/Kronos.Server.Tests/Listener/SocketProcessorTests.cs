using System.Threading.Tasks;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Server.Listener;
using NSubstitute;
using XGain.Sockets;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class SocketProcessorTests
    {
        //[Fact]
        //public async Task ProcessSocketConnection_ReceivesCorrectValue()
        //{
        //    var requestProcessorMock = Substitute.For<IRequestMapper>();
        //    var clientSocketMock = Substitute.For<ISocket>();

        //    clientSocketMock.BufferSize.Returns(65535);
        //    clientSocketMock.Connected.Returns(true);
        //    byte[] packageBytes = new byte[clientSocketMock.BufferSize];
        //    byte[] sizeBytes = new byte[sizeof(int)];

        //    byte[] requestTypeBytes = SerializationUtils.Serialize(RequestType.Get);
        //    clientSocketMock.Receive(Arg.Any<byte[]>()).Returns(sizeof(int), requestTypeBytes.Length).AndDoes(x =>
        //    {
        //        for (int i = 0; i < sizeof(int); i++)
        //        {
        //            x[i] = requestTypeBytes[i];
        //        }
        //    }).AndDoes(x =>
        //    {
        //        for (int i = 0; i < requestTypeBytes.Length; i++)
        //        {
        //            x[i] = requestTypeBytes[sizeof(int) + i];
        //        }
        //    });

        //    //requestProcessorMock.Setup(
        //    //    x =>
        //    //        x.ProcessRequest(It.IsAny<byte[]>(), It.IsAny<RequestType>())).Throws(new TaskCanceledException());

        //    SocketProcessor p = new SocketProcessor();
        //    await p.ProcessSocketConnectionAsync(clientSocketMock);

        //    clientSocketMock.Received(4).Receive(Arg.Any<byte[]>());
        //    clientSocketMock.Received(4).Send(Arg.Any<byte[]>());
        //}
    }
}
