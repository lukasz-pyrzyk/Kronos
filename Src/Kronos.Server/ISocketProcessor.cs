using System.Net.Sockets;
using Kronos.Core.Messages;

namespace Kronos.Server
{
    public interface ISocketProcessor
    {
        Request ReceiveRequest(Socket client);

        void SendResponse(Socket client, Response response);
    }
}
