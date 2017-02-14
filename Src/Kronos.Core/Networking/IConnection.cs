using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Requests;

namespace Kronos.Core.Networking
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
        Task<byte[]> SendAsync<TRequest>(TRequest request, ServerConfig server) where TRequest : IRequest;
    }
}
