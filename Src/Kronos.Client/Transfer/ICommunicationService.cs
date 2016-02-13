using Kronos.Core.Requests;

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
        /// <returns>Status code of request</returns>
        byte[] SendToNode(Request request);
    }
}
