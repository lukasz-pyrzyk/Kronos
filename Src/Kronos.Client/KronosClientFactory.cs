using System.IO;
using Kronos.Core.Configuration;
using Newtonsoft.Json;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient FromFile(string configFilePath)
        {
            string configContent = File.ReadAllText(configFilePath);

            KronosConfig config = JsonConvert.DeserializeObject<KronosConfig>(configContent);

            return new KronosClient(config);
        }

        public static IKronosClient FromLocalhost(int port)
        {
            return FromDomain("localhost", port);
        }

        public static IKronosClient FromDomain(string domain, int port)
        {
            return CreateInternal(domain, null, port);
        }

        public static IKronosClient FromIp(string ip, int port)
        {
            return CreateInternal(null, ip, port);
        }

        private static IKronosClient CreateInternal(string domain, string ip, int port)
        {
            var config = new KronosConfig
            {
                ClusterConfig = new ClusterConfig
                {
                    Servers = new[] { new ServerConfig { Domain = domain, Ip = ip, Port = port } }
                }
            };

            return new KronosClient(config);
        }
    }
}