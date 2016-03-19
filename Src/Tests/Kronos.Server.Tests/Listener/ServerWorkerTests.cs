using System;
using System.Net;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.RequestProcessing;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using Kronos.Server.Listener;
using Moq;
using XGain;
using XGain.Processing;
using XGain.Sockets;
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

        [Fact]
        public void Dispose_ClearsStorage()
        {
            var requestProcessorMock = new Mock<IRequestMapper>();
            var storageMock = new Mock<IStorage>();
            var serverMock = new Mock<IServer>();

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);
            worker.Dispose();

            storageMock.Verify(x => x.Clear(), Times.Once);
            serverMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
