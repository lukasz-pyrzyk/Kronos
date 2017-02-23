using System.IO;
using System.Net;
using Kronos.Core.Configuration;
using Kronos.Core.Networking;
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
            IPAddress ip = EndpointUtils.GetIPAsync().Result;
            return FromDomain(ip.ToString(), port);
        }

        public static IKronosClient FromDomain(string domain, int port)
        {
            return CreateInternal(domain, null, port);
        }

        public static IKronosClient FromIp(string ip, int port)
        {
            return CreateInternal(null, ip, port);
        }


        public static IKronosClient FromConnectionString(string[] connectionStrings)
        {
            ServerConfig[] servers = new ServerConfig[connectionStrings.Length];

            for (int i = 0; i < connectionStrings.Length; i++)
            {
                string con = connectionStrings[i];
                const int port = 5000;

                servers[i] = new ServerConfig { Ip = con, Port = port };
            }

            return new KronosClient(new KronosConfig { ClusterConfig = new ClusterConfig { Servers = servers } });
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