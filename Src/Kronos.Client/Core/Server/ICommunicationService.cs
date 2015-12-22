using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Requests;

namespace Kronos.Client.Core.Server
{
    /// <summary>
    /// Communication layer between client and server
    /// </summary>
    public interface ICommunicationService
    {
        /// <summary>
        /// Send request to the server
        /// </summary>
        /// <param name="request">Model of the request data to save in server cache</param>
        /// <returns>Status code of request</returns>
        RequestStatusCode SendToNode(InsertRequest request);
    }
}
