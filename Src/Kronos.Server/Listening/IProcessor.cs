using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Server.EventArgs;

namespace Kronos.Server.Listening
{
    public interface IProcessor<T> where T : RequestArgs
    {
        Task<T> ProcessSocketConnectionAsync(Socket socket);
    }
}
