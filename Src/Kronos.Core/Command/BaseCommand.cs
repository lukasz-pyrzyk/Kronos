using System.Net.Sockets;
using Kronos.Core.Storage;

namespace Kronos.Core.Command
{
    public abstract class BaseCommand
    {
        public abstract void ProcessRequest(Socket socket, byte[] requestBytes, IStorage storage);
        
        protected static void SendToClient(Socket clientSocket, byte[] buffer)
        {
            clientSocket.Send(buffer, buffer.Length, SocketFlags.None);
        }
    }
}
