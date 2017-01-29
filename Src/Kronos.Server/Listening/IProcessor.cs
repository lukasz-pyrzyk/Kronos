using System.Net.Sockets;
using Kronos.Server.EventArgs;

namespace Kronos.Server.Listening
{
    public interface IProcessor
    {
        RequestArgs ReceiveRequest(Socket client);
    }
}
