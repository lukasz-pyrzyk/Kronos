using Kronos.Core.Requests;

namespace Kronos.Core.Communication
{
    /// <summary>
    /// Communication layer between client and server
    /// </summary>
    public interface IClientServerConnection
    {
        /// <summary>
        /// Send request to the server
        /// </summary>
        /// <param name="type">Type of the request</param>
        /// <param name="request">Model of the request model</param>
        /// <returns>Status code of request</returns>
        byte[] Send<TRequest>(TRequest request) where TRequest : IRequest;
    }
}
