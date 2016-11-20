using Kronos.Core.Requests;
using XGain.Sockets;

namespace Kronos.Core.Processors
{
    public interface IRequestProcessor
    {
        void HandleIncomingRequest(RequestType requestType, byte[] request, int received, ISocket client);
    }
}
