using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Requests;

namespace Kronos.Core.Processing
{
    public interface IRequestProcessor
    {
        Task HandleAsync(RequestType requestType, byte[] request, int received, Socket client);
    }
}
