using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;

namespace Kronos.Core.Networking
{
    /// <summary>
    /// Communication layer between client and server
    /// </summary>
    public interface IConnection
    {
        Task<Response> SendAsync(Request request, ServerConfig server);
    }
}
