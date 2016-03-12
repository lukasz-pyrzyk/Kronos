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
        public void StartListening_WorksCorrectly()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var storageMock = new Mock<IStorage>();
            var socketMock = new Mock<ISocket>();
            var clientSocketMock = new Mock<ISocket>();
            var serverMock = new Mock<IServer>();

            socketMock.Setup(x => x.BufferSize).Returns(65535);
            clientSocketMock.Setup(x => x.BufferSize).Returns(65535);
            clientSocketMock.Setup(x => x.Connected).Returns(true);
            byte[] packageBytes = new byte[socketMock.Object.BufferSize];
            byte[] sizeBytes = new byte[sizeof(int)];

            socketMock.Setup(x => x.Accept()).Returns(clientSocketMock.Object);

            byte[] requestTypeBytes = SerializationUtils.Serialize(RequestType.GetRequest);
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
                    x.ProcessRequest(It.IsAny<ISocket>(), It.IsAny<byte[]>(), It.IsAny<RequestType>(),
                        It.IsAny<IStorage>())).Throws(new TaskCanceledException());

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);
            try
            {
                worker.StartListening();
            }
            catch (TaskCanceledException)
            {
            }

            socketMock.Verify(x => x.Accept(), Times.Once);
            clientSocketMock.Verify(x => x.Receive(It.IsAny<byte[]>()), Times.Exactly(4));
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.Ok);
            clientSocketMock.Verify(x => x.Send(responseBytes), Times.Exactly(2));

            requestProcessorMock.Verify(x =>
                x.ProcessRequest(clientSocketMock.Object, requestTypeBytes, RequestType.GetRequest, storageMock.Object),
                Times.Once);
            clientSocketMock.Verify(x => x.Shutdown(SocketShutdown.Both), Times.Once);
        }

        [Fact]
        public void StartListening_CatchesSocketExceptionFromShuttingDownClient()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var storageMock = new Mock<IStorage>();
            var socketMock = new Mock<ISocket>();
            var clientSocketMock = new Mock<ISocket>();
            var serverMock = new Mock<IServer>();


            socketMock.Setup(x => x.BufferSize).Returns(65535);
            clientSocketMock.Setup(x => x.BufferSize).Returns(65535);
            clientSocketMock.Setup(x => x.Connected).Returns(true);
            clientSocketMock.Setup(x => x.Shutdown(It.IsAny<SocketShutdown>())).Throws(new SocketException());
            byte[] packageBytes = new byte[socketMock.Object.BufferSize];
            byte[] sizeBytes = new byte[sizeof(int)];

            socketMock.Setup(x => x.Accept()).Returns(clientSocketMock.Object);

            byte[] requestTypeBytes = SerializationUtils.Serialize(RequestType.GetRequest);
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
                    x.ProcessRequest(It.IsAny<ISocket>(), It.IsAny<byte[]>(), It.IsAny<RequestType>(),
                        It.IsAny<IStorage>())).Throws(new TaskCanceledException());

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);
            try
            {
                worker.StartListening();
            }
            catch (TaskCanceledException)
            {
            }

            socketMock.Verify(x => x.Accept(), Times.Once);
            clientSocketMock.Verify(x => x.Receive(It.IsAny<byte[]>()), Times.Exactly(4));
            byte[] responseBytes = SerializationUtils.Serialize(RequestStatusCode.Ok);
            clientSocketMock.Verify(x => x.Send(responseBytes), Times.Exactly(2));

            requestProcessorMock.Verify(x =>
                x.ProcessRequest(clientSocketMock.Object, requestTypeBytes, RequestType.GetRequest, storageMock.Object),
                Times.Once);
            clientSocketMock.Verify(x => x.Shutdown(SocketShutdown.Both), Times.Once);
        }

        [Fact]
        public void StartListening_DoesntShutdownWhenClientIsNotConnected()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var storageMock = new Mock<IStorage>();
            var socketMock = new Mock<ISocket>();
            var clientSocketMock = new Mock<ISocket>();
            var serverMock = new Mock<IServer>();


            socketMock.Setup(x => x.BufferSize).Returns(65535);
            clientSocketMock.Setup(x => x.BufferSize).Returns(65535);
            clientSocketMock.Setup(x => x.Connected).Returns(false);
            byte[] packageBytes = new byte[socketMock.Object.BufferSize];
            byte[] sizeBytes = new byte[sizeof(int)];

            socketMock.Setup(x => x.Accept()).Returns(clientSocketMock.Object);

            byte[] requestTypeBytes = SerializationUtils.Serialize(RequestType.GetRequest);
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
                    x.ProcessRequest(It.IsAny<ISocket>(), It.IsAny<byte[]>(), It.IsAny<RequestType>(),
                        It.IsAny<IStorage>())).Throws(new TaskCanceledException());

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);
            try
            {
                worker.StartListening();
            }
            catch (TaskCanceledException)
            {
            }

            clientSocketMock.Verify(x => x.Shutdown(It.IsAny<SocketShutdown>()), Times.Never);
        }

        [Fact]
        public void StartListening_ShutsdownAndDisposesServerWhenExceptionOccurs()
        {
            var requestProcessorMock = new Mock<IRequestProcessor>();
            var storageMock = new Mock<IStorage>();
            var socketMock = new Mock<ISocket>();
            var clientSocketMock = new Mock<ISocket>();
            var serverMock = new Mock<IServer>();


            clientSocketMock.Setup(x => x.Connected).Returns(true);
            socketMock.Setup(x => x.Accept()).Returns(clientSocketMock.Object);

            clientSocketMock.Setup(x => x.Receive(It.IsAny<byte[]>())).Throws(new Exception());

            ServerWorker worker = new ServerWorker(requestProcessorMock.Object, storageMock.Object, serverMock.Object);
            worker.StartListening();

            clientSocketMock.Verify(x => x.Shutdown(It.IsAny<SocketShutdown>()), Times.Once);
            socketMock.Verify(x => x.Shutdown(It.IsAny<SocketShutdown>()), Times.Once);
            socketMock.Verify(x => x.Dispose(), Times.Once);
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
