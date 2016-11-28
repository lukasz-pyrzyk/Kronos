using Kronos.Core.Processing;
using Kronos.Core.Storage;
using Kronos.Server.Listener;
using NSubstitute;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class ServerWorkerTests
    {
        [Fact]
        public void Ctor_AssignsServices()
        {
            var requestProcessorMock = Substitute.For<IRequestProcessor>();
            var storageMock = Substitute.For<IStorage>();
            var serverMock = Substitute.For<IListener>();

            ServerWorker worker = new ServerWorker(requestProcessorMock, storageMock, serverMock);

            Assert.NotNull(worker);
            Assert.Equal(worker.Storage, storageMock);
        }

        [Fact]
        public void Dispose_StopsServer()
        {
            var requestProcessorMock = Substitute.For<IRequestProcessor>();
            var storageMock = Substitute.For<IStorage>();
            var serverMock = Substitute.For<IListener>();

            ServerWorker worker = new ServerWorker(requestProcessorMock, storageMock, serverMock);
            worker.Dispose();

            serverMock.Received(1).Dispose();
        }
    }
}
