using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Kronos.Client.Transfer;
using Kronos.Core.Exceptions;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Moq;
using XGain.Sockets;
using Xunit;

namespace Kronos.Client.Tests.Transfer
{
    public class SocketCommunicationServiceTests
    {
        [Fact]
        public async Task SendToServer_WorksCorrect()
        {
            var socketMock = new Mock<ISocket>();
            socketMock.Setup(x => x.BufferSize).Returns(65535);

            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;
            InsertRequest request = new InsertRequest("key", Encoding.UTF8.GetBytes("lorem ipsum"), DateTime.Today);
            byte[] requestBytes = SerializationUtils.Serialize(request);
            byte[] requestTypeBytes = SerializationUtils.Serialize(request.RequestType);
            byte[] statusCodeBytes = SerializationUtils.Serialize(expectedStatusCode);
            byte[] intSizebytes = new byte[sizeof(int)];
            byte[] informationTransBuffer = new byte[socketMock.Object.BufferSize];

            socketMock.Setup(x => x.Receive(It.IsAny<byte[]>()))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(statusCodeBytes.Length);

            socketMock.Setup(x => x.Receive(intSizebytes))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(sizeof(int));

            int reponseSize = sizeof(RequestStatusCode);
            socketMock.Setup(x => x.Receive(informationTransBuffer))
                .Callback<byte[]>(x =>
                {
                    for (int i = 0; i < reponseSize; i++)
                    {
                        x[i] = statusCodeBytes[sizeof(int) + i];
                    }
                })
                .Returns(reponseSize);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 5000);

            SocketCommunicationService service = new SocketCommunicationService(endpoint, () => socketMock.Object);
            byte[] response = await service.SendToServerAsync(request);

            Assert.Equal(SerializationUtils.Deserialize<RequestStatusCode>(response), expectedStatusCode);

            socketMock.Verify(x => x.Send(requestTypeBytes), Times.AtLeastOnce);
            socketMock.Verify(x => x.Send(requestBytes), Times.AtLeastOnce);
            socketMock.Verify(x => x.Receive(statusCodeBytes), Times.AtLeast(2));
            socketMock.Verify(x => x.Dispose(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task SendToServer_ThrowsExceptionWhenIsMismatchInReceiveBytesNumber()
        {
            var socketMock = new Mock<ISocket>();
            socketMock.Setup(x => x.BufferSize).Returns(65535);

            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;
            InsertRequest request = new InsertRequest("key", Encoding.UTF8.GetBytes("lorem ipsum"), DateTime.Today);
            byte[] requestBytes = SerializationUtils.Serialize(request);
            byte[] requestTypeBytes = SerializationUtils.Serialize(request.RequestType);
            byte[] statusCodeBytes = SerializationUtils.Serialize(expectedStatusCode);
            byte[] intSizebytes = new byte[sizeof(int)];
            byte[] informationTransBuffer = new byte[socketMock.Object.BufferSize];

            socketMock.Setup(x => x.Receive(It.IsAny<byte[]>()))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(statusCodeBytes.Length);

            socketMock.Setup(x => x.Receive(intSizebytes))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(sizeof(int));

            int reponseSize = sizeof(RequestStatusCode);
            socketMock.Setup(x => x.Receive(informationTransBuffer))
                .Callback<byte[]>(x =>
                {
                    for (int i = 0; i < reponseSize; i++)
                    {
                        x[i] = statusCodeBytes[sizeof(int) + i];
                    }
                })
                .Returns(short.MaxValue);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 5000);

            SocketCommunicationService service = new SocketCommunicationService(endpoint, () => socketMock.Object);

            Exception transferException = null;
            try
            {
                await service.SendToServerAsync(request);
            }
            catch (Exception ex)
            {
                transferException = ex;
            }

            Assert.NotNull(transferException);
            Assert.IsType<KronosCommunicationException>(transferException);
            Assert.IsType<KronosCommunicationException>(transferException.InnerException);
            Assert.Equal(transferException.Message, "Invalid tcp error. Socket has received more bytes than was specified.");

            socketMock.Verify(x => x.Send(requestTypeBytes), Times.Exactly(SocketCommunicationService.RetryCount + 1));
            socketMock.Verify(x => x.Send(requestBytes), Times.Exactly(SocketCommunicationService.RetryCount + 1));
            socketMock.Verify(x => x.Receive(statusCodeBytes), Times.Exactly((SocketCommunicationService.RetryCount + 1) * 2));
            socketMock.Verify(x => x.Dispose(), Times.Exactly(SocketCommunicationService.RetryCount + 1));
        }

        [Fact]
        public async Task SendToServer_ThrowsExceptionDuringDisposing()
        {
            var socketMock = new Mock<ISocket>();
            socketMock.Setup(x => x.BufferSize).Returns(65535);
            string exceptionMessage = "fake exception";
            socketMock.Setup(x => x.Dispose()).Throws(new Exception(exceptionMessage));

            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;
            InsertRequest request = new InsertRequest("key", Encoding.UTF8.GetBytes("lorem ipsum"), DateTime.Today);
            byte[] requestBytes = SerializationUtils.Serialize(request);
            byte[] requestTypeBytes = SerializationUtils.Serialize(request.RequestType);
            byte[] statusCodeBytes = SerializationUtils.Serialize(expectedStatusCode);
            byte[] intSizebytes = new byte[sizeof(int)];
            byte[] informationTransBuffer = new byte[socketMock.Object.BufferSize];

            socketMock.Setup(x => x.Receive(It.IsAny<byte[]>()))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(statusCodeBytes.Length);

            socketMock.Setup(x => x.Receive(intSizebytes))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(sizeof(int));

            int reponseSize = sizeof(RequestStatusCode);
            socketMock.Setup(x => x.Receive(informationTransBuffer))
                .Callback<byte[]>(x =>
                {
                    for (int i = 0; i < reponseSize; i++)
                    {
                        x[i] = statusCodeBytes[sizeof(int) + i];
                    }
                })
                .Returns(short.MaxValue);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 5000);

            SocketCommunicationService service = new SocketCommunicationService(endpoint, () => socketMock.Object);

            Exception transferException = null;
            try
            {
                await service.SendToServerAsync(request);
            }
            catch (Exception ex)
            {
                transferException = ex;
            }

            Assert.NotNull(transferException);
            Assert.IsType<Exception>(transferException);
            Assert.Equal(transferException.Message, exceptionMessage);

            socketMock.Verify(x => x.Send(requestTypeBytes), Times.Exactly(SocketCommunicationService.RetryCount + 1));
            socketMock.Verify(x => x.Send(requestBytes), Times.Exactly(SocketCommunicationService.RetryCount + 1));
            socketMock.Verify(x => x.Receive(statusCodeBytes), Times.Exactly((SocketCommunicationService.RetryCount + 1) * 2));
            socketMock.Verify(x => x.Dispose(), Times.Exactly(SocketCommunicationService.RetryCount + 1));
        }

        [Fact]
        public async Task SendToServer_WorksCorrectWithSocketExceptionOnDisposing()
        {
            var socketMock = new Mock<ISocket>();
            socketMock.Setup(x => x.BufferSize).Returns(65535);
            socketMock.Setup(x => x.Dispose()).Throws(new SocketException());

            RequestStatusCode expectedStatusCode = RequestStatusCode.Ok;
            InsertRequest request = new InsertRequest("key", Encoding.UTF8.GetBytes("lorem ipsum"), DateTime.Today);
            byte[] requestBytes = SerializationUtils.Serialize(request);
            byte[] requestTypeBytes = SerializationUtils.Serialize(request.RequestType);
            byte[] statusCodeBytes = SerializationUtils.Serialize(expectedStatusCode);
            byte[] intSizebytes = new byte[sizeof(int)];
            byte[] informationTransBuffer = new byte[socketMock.Object.BufferSize];

            socketMock.Setup(x => x.Receive(It.IsAny<byte[]>()))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(statusCodeBytes.Length);

            socketMock.Setup(x => x.Receive(intSizebytes))
                .Callback<byte[]>(package =>
                {
                    for (int i = 0; i < package.Length; i++)
                    {
                        package[i] = statusCodeBytes[i];
                    }
                })
                .Returns(sizeof(int));

            int reponseSize = sizeof(RequestStatusCode);
            socketMock.Setup(x => x.Receive(informationTransBuffer))
                .Callback<byte[]>(x =>
                {
                    for (int i = 0; i < reponseSize; i++)
                    {
                        x[i] = statusCodeBytes[sizeof(int) + i];
                    }
                })
                .Returns(reponseSize);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 5000);

            SocketCommunicationService service = new SocketCommunicationService(endpoint, () => socketMock.Object);
            await service.SendToServerAsync(request);

            socketMock.Verify(x => x.Send(requestTypeBytes), Times.Once);
            socketMock.Verify(x => x.Send(requestBytes), Times.Once);
            socketMock.Verify(x => x.Receive(statusCodeBytes), Times.Exactly(2));
            socketMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
