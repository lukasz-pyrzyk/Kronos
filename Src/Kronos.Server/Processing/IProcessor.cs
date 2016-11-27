using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Server.EventArgs;

namespace Kronos.Server.Processing
{
    public interface IProcessor<T> where T : MessageArgs
    {
        Task<T> ProcessSocketConnectionAsync(Socket socket);
    }
}
