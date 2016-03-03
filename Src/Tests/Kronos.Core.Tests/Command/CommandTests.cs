using System.Net.Sockets;
using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using Moq;
using Xunit;

namespace Kronos.Core.Tests.Command
{
    public class CommandTests
    {
        [Fact]
        public void CanCreateInstanceOfFakeCommand()
        {
            var communicationServiceMock = new Mock<IClientServerConnection>();

            FakeCommand command = new FakeCommand(communicationServiceMock.Object, null);

            Assert.NotNull(command);
        }

        internal class FakeCommand : BaseCommand
        {
            public FakeCommand(IClientServerConnection service, Request request) : base(service, request)
            {
            }

            public override void ProcessRequest(Socket socket, byte[] requestBytes, IStorage storage)
            {
            }
        }
    }
}
