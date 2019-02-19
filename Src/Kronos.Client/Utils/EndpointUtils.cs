using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Client.Utils
{
    public static class EndpointUtils
    {
        public static async Task<IPAddress> GetIpAsync(string hostName)
        {
            var host = await Dns.GetHostEntryAsync(hostName);
            IPAddress[] addresses = host.AddressList
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToArray();

            if (addresses.Length > 1)
            {
                Trace.TraceInformation($"Found more local network interfaces, choosing {addresses.First()}");
            }

            return addresses.First();
        }
    }
}
