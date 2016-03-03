using System.Net.Sockets;

namespace Kronos.Server.Listener
{
    public interface IServerWorker
    {
        void StartListening(Socket server);
    }
}
