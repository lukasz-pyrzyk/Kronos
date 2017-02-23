using System.Net.Sockets;

namespace Kronos.Server.Listening
{
    public interface IProcessor
    {
        Request ReceiveRequest(Socket client);

        void SendResponse(Socket client, Response response);
    }
}
