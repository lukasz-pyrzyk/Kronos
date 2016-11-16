using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Core.Communication
{
    public static class EndpointUtils
    {
        public static async Task<IPAddress> GetIPAsync(string hostName = "localhost")
        {
            var host = await Dns.GetHostEntryAsync(hostName);

            return host.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).First();
        }
    }
}
