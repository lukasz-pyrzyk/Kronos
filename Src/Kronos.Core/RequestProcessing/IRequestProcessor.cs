using System.Net.Sockets;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.RequestProcessing
{
    public interface IRequestProcessor
    {
        void ProcessRequest(Socket clientSocket, byte[] requestBytes, RequestType type, IStorage storage);
    }
}
