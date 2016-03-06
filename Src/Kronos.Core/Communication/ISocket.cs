using System;
using System.Net;
using System.Net.Sockets;

namespace Kronos.Core.Communication
{
    public interface ISocket : IDisposable
    {
        int BufferSize { get;}
        bool Connected { get; }
        Socket InternalSocket { get; }
        EndPoint LocalEndPoint { get; }
        EndPoint RemoteEndPoint { get; }

        ISocket Accept();
        void Bind(IPEndPoint localEndPoint);
        void Connect(IPEndPoint remoteEndPoint);
        void Listen(int backlog);
        int Receive(byte[] buffer);
        int Send(byte[] buffer);
        void Shutdown(SocketShutdown how);
    }
}
