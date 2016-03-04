using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Server.Listener;
using Moq;
using Xunit;

namespace Kronos.Server.Tests.Listener
{
    public class TcpServerTests
    {
        [Fact]
        public void Ctor_InisializeServer()
        {
            var worker = new Mock<IServerWorker>();

            TcpServer server = new TcpServer(worker.Object, 5000);

            Assert.NotNull(server);
        }

        [Fact]
        public void Start_StartsListening()
        {
            var worker = new Mock<IServerWorker>();

            TcpServer server = new TcpServer(worker.Object, 6000);
            server.Start();

            worker.Verify(x => x.StartListening(It.IsAny<Socket>()), Times.Exactly(1));
            Assert.NotNull(server);
        }

        [Fact]
        public void Dispose_CanDisposeElement()
        {
            var worker = new Mock<IServerWorker>();
            TcpServer server = new TcpServer(worker.Object, 7000);

            server.Dispose();

            Assert.True(server.IsDisposed);
        }
    }
}
