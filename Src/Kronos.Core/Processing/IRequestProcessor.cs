using System.Net.Sockets;
using Kronos.Core.Requests;

namespace Kronos.Core.Processing
{
    public interface IRequestProcessor
    {
        void HandleIncomingRequest(RequestType requestType, byte[] request, int received, Socket client);
    }
}
