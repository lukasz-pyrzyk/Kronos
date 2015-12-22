using Kronos.Shared.Network.Models;

namespace Kronos.Client.Core.Server
{
    /// <summary>
    /// Communication layer between client and server
    /// </summary>
    public interface IKronosCommunicationService
    {
        /// <summary>
        /// Send request to the server
        /// </summary>
        /// <param name="request">Model of the request data to save in server cache</param>
        /// <returns>Status code of request</returns>
        RequestStatusCode SendToNode(SocketRequest request);
    }
}
