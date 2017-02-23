using System.Net.Sockets;

namespace Kronos.Server.Listening
{
    public interface IProcessor
    {
        RequestArg ReceiveRequest(Socket client);

        void SendResponse(Socket client, Response response);
    }
}
