using Kronos.Client.Command;
using Kronos.Client.Transfer;
using Kronos.Core.Requests;
using Moq;
using Xunit;

namespace Kronos.Client.Tests.Command
{
    public class CommandTests
    {
        [Fact]
        public void CanCreateInstanceOfFakeCommand()
        {
            var communicationServiceMock = new Mock<ICommunicationService>();

            FakeCommand command = new FakeCommand(communicationServiceMock.Object, null);

            Assert.NotNull(command);
        }

        internal class FakeCommand : BaseCommand
        {
            public FakeCommand(ICommunicationService service, Request request) : base(service, request)
            {
            }
        }
    }
}
