using Kronos.Core.Requests;

namespace Kronos.Core.Network
{
    /// <summary>
    /// Communication layer between client and server
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Send request to the server
        /// </summary>
        /// <param name="request">Request to process on the server</param>
        /// <returns>Status code of request</returns>
        byte[] Send<TRequest>(TRequest request) where TRequest : IRequest;
    }
}
