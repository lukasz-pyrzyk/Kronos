using System;
using System.Net.Sockets;
using Kronos.Core.Messages;

namespace Kronos.Server
{
    public interface ISocketProcessor
    {
        Request ReceiveRequest(Socket client, Memory<byte> buffer);

        void SendResponse(Socket client, Response response);
    }
}
