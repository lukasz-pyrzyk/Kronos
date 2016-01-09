using Kronos.Client.Core.Server;
using Kronos.Shared.Configuration;

namespace Kronos.Client.Core
{
    public static class KronosClientFactory
    {
        public static IKronosClient CreateClient()
        {
            IServerConfiguration configuration = new ServerConfiguration();
            configuration.NodesConfiguration.Add(new NodeConfiguration("192.168.43.75", 7));

            return new KronosClient(new SocketCommunicationService(), configuration);
        }
    }
}