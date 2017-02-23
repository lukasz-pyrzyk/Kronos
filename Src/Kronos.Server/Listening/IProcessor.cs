using System.Net.Sockets;
using Kronos.Core.Messages;

namespace Kronos.Server.Listening
{
    public interface IProcessor
    {
        Request ReceiveRequest(Socket client);

        void SendResponse(Socket client, Response response);
    }
}
