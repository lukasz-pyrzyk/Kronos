using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Requests;

namespace Kronos.Client.Core.Server
{
    public class KronosCommunicationServiceSocket : IKronosCommunicationService
    {
        public RequestStatusCode SendToNode(InsertRequest request)
        {
            return RequestStatusCode.Ok;
        }
    }
}
