using System;
using System.Net;
using System.Net.Sockets;

namespace Kronos.Core.Communication
{
    public interface ISocket : IDisposable
    {
        int BufferSize { get;}
        EndPoint LocalEndPoint { get; }
        EndPoint RemoteEndPoint { get; }
        bool Connected { get; }
        Socket InternalSocket { get; }
        ISocket Accept();
        void Bind(IPEndPoint localEndPoint);
        void Connect(IPEndPoint remoteEndPoint);
        void Listen(int backlog);
        void Shutdown(SocketShutdown how);
        int Receive(byte[] buffer);
        int Send(byte[] buffer);
    }
}
