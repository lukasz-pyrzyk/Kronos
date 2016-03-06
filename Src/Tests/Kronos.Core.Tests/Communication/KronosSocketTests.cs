using System.Net;
using System.Net.Sockets;
using Kronos.Core.Communication;
using Xunit;

namespace Kronos.Core.Tests.Communication
{
    public class KronosSocketTests
    {
        [Fact]
        public void Ctor_CanInitialize()
        {
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, false, 0);

            Assert.NotNull(socket);
            Assert.NotNull(socket.InternalSocket);
        }

        [Fact]
        public void Ctor_CanInitializeWithDefaultProperties()
        {
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork);

            Assert.NotNull(socket);
            Assert.NotNull(socket.InternalSocket);
        }

        [Fact]
        public void Ctor_CanInitializeWithSocket()
        {
            Socket internalSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            ISocket socket = new KronosSocket(internalSocket);

            Assert.NotNull(socket);
            Assert.Equal(socket.InternalSocket, internalSocket);
        }

        [Fact]
        public void Ctor_ContainsBufferSize()
        {
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork);

            Assert.NotEqual(socket.BufferSize, default(int));
        }

        [Fact]
        public void Connected_PointsToSocket()
        {
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork);

            Assert.Equal(socket.Connected, socket.InternalSocket.Connected);
        }

        [Fact]
        public void LocalEndpoint_PointsToSocket()
        {
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork);

            Assert.Equal(socket.LocalEndPoint, socket.InternalSocket.LocalEndPoint);
        }

        [Fact]
        public void RemoteEndpoint_PointsToSocket()
        {
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork);
            
            Assert.Equal(socket.RemoteEndPoint, socket.InternalSocket.RemoteEndPoint);
        }
    }
}
