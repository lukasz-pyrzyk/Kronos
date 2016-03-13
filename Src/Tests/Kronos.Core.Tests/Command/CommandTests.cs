using System.Text;
using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Storage;
using Moq;
using XGain.Sockets;
using Xunit;

namespace Kronos.Core.Tests.Command
{
    public class CommandTests
    {
        [Fact]
        public void CanCreateInstanceOfFakeCommand()
        {
            FakeCommand command = new FakeCommand();

            Assert.NotNull(command);
        }

        [Fact]
        public void SendToClient_SendsBufferToClient()
        {
            var socketMock = new Mock<ISocket>();
            byte[] buffer = Encoding.UTF8.GetBytes("lorem ipsum");

            FakeCommand command = new FakeCommand();
            command.SendToClientFake(socketMock.Object, buffer);

            socketMock.Verify(x => x.Send(buffer), Times.Exactly(1));
        }

        internal class FakeCommand : BaseCommand
        {
            public override void ProcessRequest(ISocket socket, byte[] requestBytes, IStorage storage)
            {
            }

            public void SendToClientFake(ISocket clientSocket, byte[] buffer)
            {
                clientSocket.Send(buffer);
            }
        }
    }
}
