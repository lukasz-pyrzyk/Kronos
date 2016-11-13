using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kronos.Core.Communication
{
    public static class IPAddressUtils
    {
        public static async Task<IPAddress> GetLocalAsync()
        {
            var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            throw new Exception("Local IP Address Not Found!");
        }
    }
}
