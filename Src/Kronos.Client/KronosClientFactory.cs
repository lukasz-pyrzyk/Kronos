using System.IO;
using System.Net;
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

        public static IKronosClient FromLocalhost(int port, string login = Settings.DefaultLogin, string password = Settings.DefaultPassword)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            return FromIp(ip.ToString(), port, login, password);
        }

        public static IKronosClient FromDomain(string domain, int port, string login = Settings.DefaultLogin, string password = Settings.DefaultPassword)
        {
            return CreateInternal(domain, null, port, login, password);
        }

        public static IKronosClient FromIp(string ip, int port, string login = Settings.DefaultLogin, string password = Settings.DefaultPassword)
        {
            return CreateInternal(null, ip, port, login, password);
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