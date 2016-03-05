using Kronos.Core.Communication;
using Kronos.Core.Storage;

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
