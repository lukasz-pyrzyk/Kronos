using System.Threading.Tasks;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Server.Listener;
using Moq;
using XGain;
using XGain.Processing;
using XGain.Sockets;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class SocketProcessorTests
    {
        [Fact]
        public async Task ProcessSocketConnection_ReceivesCorrectValue()
        {
            var request = new GetRequest("key");
            byte[] package = SerializationUtils.Serialize(request.RequestType);
            byte[] sizeBuffor = new byte[sizeof(int)];
            byte[] requestSize = new byte[package.Length - sizeBuffor.Length];
            var socketMock = new Mock<ISocket>();
            socketMock.SetupSequence(x => x.BufferSize).Returns(requestSize.Length);

            socketMock.Setup(x => x.ReceiveAsync(sizeBuffor))
                .Returns(Task.FromResult(sizeBuffor.Length))
                .Callback<byte[]>(x =>
                {
                    for (int i = 0; i < sizeBuffor.Length; i++)
                    {
                        x[i] = package[i];
                    }
                });

            socketMock.Setup(x => x.ReceiveAsync(requestSize))
                .Returns(Task.FromResult(requestSize.Length))
                .Callback<byte[]>(x =>
                {
                    for (int i = 0; i < requestSize.Length; i++)
                    {
                        x[i] = package[i + sizeBuffor.Length];
                    }
                });

            Message msg = new Message();
            IProcessor proc = new SocketProcessor();
            await proc.ProcessSocketConnection(socketMock.Object, msg);

            socketMock.Verify(x => x.ReceiveAsync(It.IsAny<byte[]>()), Times.Exactly(4));
            socketMock.Verify(x => x.SendAsync(It.IsAny<byte[]>()), Times.Exactly(2));
        }
    }
}
