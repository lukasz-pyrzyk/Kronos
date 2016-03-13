using Kronos.Core.Requests;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.RequestProcessing
{
    public interface IRequestProcessor
    {
        void ProcessRequest(ISocket clientSocket, byte[] requestBytes, RequestType type, IStorage storage);
    }
}
