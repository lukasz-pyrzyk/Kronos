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
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, 0, true);

            Assert.NotNull(socket);
        }

        [Fact]
        public void Ctor_CanInitializeWithDefaultProperties()
        {
            ISocket socket = new KronosSocket(AddressFamily.InterNetwork);

            Assert.NotNull(socket);
        }
    }
}
