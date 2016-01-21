using System.Net;
using Kronos.Shared.Requests;
using Kronos.Shared.StatusCodes;

namespace Kronos.Client.Transfer
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
        /// <param name="endPoint">Endpoint of the server node</param>
        /// <returns>Status code of request</returns>
        RequestStatusCode SendToNode(InsertRequest request, IPEndPoint endPoint);
    }
}
