using System.Net.Sockets;
using Kronos.Core.Storage;

namespace Kronos.Server.RequestProcessing
{
    public interface IRequestProcessor
    {
        void ProcessRequest(Socket clientSocket, byte[] requestBytes, IStorage storage);
    }
}
