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
            return await GetIPAsync(Dns.GetHostName());
        }

        public static async Task<IPAddress> GetIPAsync(string hostName)
        {
            var host = await Dns.GetHostEntryAsync(hostName);

            return host.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
