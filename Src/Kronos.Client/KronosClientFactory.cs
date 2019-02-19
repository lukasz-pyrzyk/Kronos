using System.Net;
using Kronos.Client.Configuration;
using Kronos.Core.Configuration;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient FromLocalhost(int port, string login = null, string password = null)
        {
            var ip = IPAddress.Parse("127.0.0.1");
            return FromIp(ip.ToString(), port, login, password);
        }

        public static IKronosClient FromDomain(string domain, int port, string login = null, string password = null)
        {
            return CreateInternal(domain, null, port, login, password);
        }

        public static IKronosClient FromIp(string ip, int port, string login = null, string password = null)
        {
            return CreateInternal(null, ip, port, login, password);
        }

        private static IKronosClient CreateInternal(string domain, string ip, int port, string login, string password)
        {
            var config = new KronosConfig
            {
                Server = new ServerConfig
                {
                    Domain = domain,
                    Ip = ip,
                    Port = port,
                    Credentials = new AuthConfig { Login = login ?? DefaultSettings.Login, Password = password ?? DefaultSettings.Password }
                }
            };

            return new KronosClient(config);
        }
    }
}