using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Newtonsoft.Json;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient CreateClient(string configFilePath)
        {
            string configContent = File.ReadAllText(configFilePath);

            KronosConfig config = JsonConvert.DeserializeObject<KronosConfig>(configContent);

            return new KronosClient(config);
        }

        public static IKronosClient CreateClient(int port)
        {
            string localIp = GetLocalIp().Result;
            return CreateClient(localIp, port);
        }

        public static IKronosClient CreateClient(string ip, int port)
        {
            var config = new KronosConfig
            {
                ClusterConfig = new ClusterConfig
                {
                    Servers = new[] { new ServerConfig { Ip = ip, Port = port } }
                }
            };

            return new KronosClient(config);
        }

        private static async Task<string> GetLocalIp()
        {
            var hosts = await Dns.GetHostAddressesAsync(Dns.GetHostName());

            string localIp = hosts.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();

            return localIp;
        }
    }
}