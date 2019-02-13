using System.IO;
using System.Net;
using Kronos.Core.Configuration;
using Kronos.Core.Messages;
using Newtonsoft.Json;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient FromLocalhost(int port, string login = Auth.DefaultLogin, string password = Auth.DefaultPassword)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
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

        private static IKronosClient CreateInternal(string domain, string ip, int port, string login, string password)
        {
            var config = new KronosConfig
            {
                Server = new ServerConfig
                {
                    Domain = domain,
                    Ip = ip,
                    Port = port,
                    Credentials = new AuthConfig { Login = login, Password = password }
                }
            };

            return new KronosClient(config);
        }
    }
}