using Kronos.Core.Requests;
using Kronos.Core.Storage;
using Kronos.Server.Listener;
using Moq;
using XGain;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class ServerWorkerTests
    {
        [Fact]
        public void Ctor_AssignsServices()
        {
            var requestProcessorMock = new Mock<IRequestMapper>();
            var storageMock = new Mock<IStorage>();
            var serverMock = new Mock<IServer>();

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);

            Assert.NotNull(worker);
            Assert.Equal(worker.Storage, storageMock.Object);
        }
    }
}
