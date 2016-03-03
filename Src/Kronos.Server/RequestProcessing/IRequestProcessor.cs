using System.Net.Sockets;
using Kronos.Server.Storage;

namespace Kronos.Server.RequestProcessing
{
    internal interface IRequestProcessor
    {
        void ProcessRequest(Socket clientSocket, byte[] requestBytes, IStorage storage);
    }
}
