using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NLog;

namespace Kronos.Core.Networking
{
    public static class EndpointUtils
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<IPAddress> GetIpAsync(string hostName)
        {
            var host = await Dns.GetHostEntryAsync(hostName);
            IPAddress[] addresses = host.AddressList
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToArray();

            if (addresses.Length > 1)
            {
                Logger.Info($"Found more local network interfaces, choosing {addresses.First()}");
            }

            return addresses.First();
        }
    }
}
