using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Storage;
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

        internal class FakeCommand : BaseCommand
        {
            public override void ProcessRequest(ISocket socket, byte[] requestBytes, IStorage storage)
            {
            }
        }
    }
}
