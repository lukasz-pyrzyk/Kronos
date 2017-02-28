using System.IO;
using System.Net;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;
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

        public static IKronosClient FromLocalhost(int port, string login = Auth.DefaultLogin, string password = Auth.DefaultPassword)
        {
            IPAddress ip = EndpointUtils.GetIPAsync().Result;
            return FromIp(ip.ToString(), port, login, password);
        }

        public static IKronosClient FromDomain(string domain, int port, string login = Auth.DefaultLogin, string password = Auth.DefaultPassword)
        {
            return CreateInternal(domain, null, port, login, password);
        }

        public static IKronosClient FromIp(string ip, int port, string login = Auth.DefaultLogin, string password = Auth.DefaultPassword)
        {
            return CreateInternal(null, ip, port, login, password);
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

        private static IKronosClient CreateInternal(string domain, string ip, int port, string login, string password)
        {
            var config = new KronosConfig
            {
                ClusterConfig = new ClusterConfig
                {
                    Servers = new[]
                    {
                        new ServerConfig
                        {
                            Domain = domain,
                            Ip = ip,
                            Port = port,
                            Credentials = new AuthConfig { Login = login, Password = password }
                        }
                    }
                }
            };

            return new KronosClient(config);
        }
    }
}