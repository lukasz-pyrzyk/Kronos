using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.RequestProcessing;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using Kronos.Server.Listener;
using Moq;
using XGain;
using XGain.Sockets;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class ServerWorkerTests
    {
        [Fact]
        public void Ctor_AssignsServices()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var storageMock = new Mock<IStorage>();
            var serverMock = new Mock<IServer>();

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);

            Assert.NotNull(worker);
            Assert.Equal(worker.Storage, storageMock.Object);
        }

        [Fact]
        public void StartListening_CallsRequestProcessor()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var storageMock = new Mock<IStorage>();
            var serverMock = new Mock<IServer>();

            
            serverMock.Setup(x => x.Start()).Returns(Task.FromResult((object)null)).Raises(x => x.OnNewMessage += null, new Message());

            IServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);
            worker.StartListening();
            
            requestProcessorMock.Verify(x => x.ProcessRequest(It.IsAny<ISocket>(), It.IsAny<byte[]>(), It.IsAny<RequestType>(), storageMock.Object), Times.Once);
        }

        [Fact]
        public void Dispose_ClearsStorage()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var storageMock = new Mock<IStorage>();
            var serverMock = new Mock<IServer>();

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);
            worker.Dispose();
            
            storageMock.Verify(x => x.Clear(), Times.Once);
            serverMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
