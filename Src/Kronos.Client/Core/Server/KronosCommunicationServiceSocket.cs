using Kronos.Shared.Network.Models;

namespace Kronos.Client.Core.Server
{
    public class KronosCommunicationServiceSocket : IKronosCommunicationService
    {
        public RequestStatusCode SendToNode(SocketRequest request)
        {
            return RequestStatusCode.Ok;
        }
    }
}
