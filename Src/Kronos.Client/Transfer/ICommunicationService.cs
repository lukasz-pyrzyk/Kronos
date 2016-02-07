using System.Net;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;

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
        /// <param name="request">Model of the request model</param>
        /// <param name="endPoint">Endpoint of the server node</param>
        /// <returns>Status code of request</returns>
        byte[] SendToNode(Request request, IPEndPoint endPoint);
    }
}
