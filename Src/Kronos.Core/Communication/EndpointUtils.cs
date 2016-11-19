using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Core.Communication
{
    public static class EndpointUtils
    {
        public static async Task<IPAddress> GetIPAsync()
        {
            return await GetIPAsync(Dns.GetHostName());
        }

        public static async Task<IPAddress> GetIPAsync(string hostNameW)
        {
            var host = await Dns.GetHostEntryAsync(Dns.GetHostName());

            return host.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
