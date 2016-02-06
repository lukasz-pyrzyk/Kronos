using System.Net;
using Kronos.Client.Transfer;
using Kronos.Core.Configuration;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient CreateClient(IPAddress host, int port)
        {
            IServerConfiguration configuration = new ServerConfiguration();
            configuration.NodesConfiguration.Add(new NodeConfiguration(host, port));

            return new KronosClient(new SocketCommunicationService(), configuration);
        }
    }
}