using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.RequestProcessing
{
    public interface IRequestProcessor
    {
        void ProcessRequest(ISocket clientSocket, byte[] requestBytes, RequestType type, IStorage storage);
    }
}
