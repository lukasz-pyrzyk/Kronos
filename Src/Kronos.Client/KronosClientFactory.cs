using System.IO;
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

        public static IKronosClient CreateClient(int port, string ip = null)
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
    }
}