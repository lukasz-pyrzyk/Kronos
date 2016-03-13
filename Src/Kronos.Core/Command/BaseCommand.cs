using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.Command
{
    public abstract class BaseCommand
    {
        public abstract void ProcessRequest(ISocket socket, byte[] requestBytes, IStorage storage);
        
        protected static void SendToClient(ISocket clientSocket, byte[] buffer)
        {
            clientSocket.Send(buffer);
        }
    }
}
