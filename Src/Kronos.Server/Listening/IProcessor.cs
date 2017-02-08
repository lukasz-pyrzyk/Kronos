using System.Net.Sockets;

namespace Kronos.Server.Listening
{
    public interface IProcessor
    {
        RequestArgs ReceiveRequest(Socket client);
    }
}
