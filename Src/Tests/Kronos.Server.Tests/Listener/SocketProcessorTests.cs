using System.Threading.Tasks;
using Kronos.Core.RequestProcessing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using Kronos.Server.Listener;
using Moq;
using XGain;
using XGain.Sockets;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class SocketProcessorTests
    {
        [Fact]
        public async Task ProcessSocketConnection_ReceivesCorrectValue()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var clientSocketMock = new Mock<ISocket>();

            clientSocketMock.Setup(x => x.BufferSize).Returns(65535);
            clientSocketMock.Setup(x => x.Connected).Returns(true);
            byte[] packageBytes = new byte[clientSocketMock.Object.BufferSize];
            byte[] sizeBytes = new byte[sizeof(int)];
            
            byte[] requestTypeBytes = SerializationUtils.Serialize(RequestType.Get);
            clientSocketMock.Setup(x => x.Receive(sizeBytes))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < sizeof(int); i++)
                    {
                        package[i] = requestTypeBytes[i];
                    }
                })
                .Returns(sizeof(int));

            int requestTypeSize = sizeof(RequestType);
            clientSocketMock.Setup(x => x.Receive(packageBytes))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < requestTypeSize; i++)
                    {
                        package[i] = requestTypeBytes[sizeof(int) + i];
                    }
                })
                .Returns(requestTypeSize);

            requestProcessorMock.Setup(
                x =>
                    x.ProcessRequest(It.IsAny<byte[]>(), It.IsAny<RequestType>())).Throws(new TaskCanceledException());

            SocketProcessor p = new SocketProcessor();
            MessageArgs msg = new MessageArgs();
            p.ProcessSocketConnection(clientSocketMock.Object, msg);

            clientSocketMock.Verify(x => x.Receive(It.IsAny<byte[]>()), Times.Exactly(4));
            clientSocketMock.Verify(x => x.Send(It.IsAny<byte[]>()), Times.Exactly(2));
        }
    }
}
