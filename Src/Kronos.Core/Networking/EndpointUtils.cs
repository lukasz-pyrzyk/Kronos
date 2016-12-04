using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Core.Networking
{
    public static class EndpointUtils
    {
        public static async Task<IPAddress> GetIPAsync()
        {
            return await GetIPAsync(Dns.GetHostName()).ConfigureAwait(false);
        }

        public static async Task<IPAddress> GetIPAsync(string hostNameW)
        {
            var host = await Dns.GetHostEntryAsync(hostNameW).ConfigureAwait(false);

            return host.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
