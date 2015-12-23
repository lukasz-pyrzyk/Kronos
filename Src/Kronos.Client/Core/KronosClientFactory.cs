using Kronos.Client.Core.Server;
using Kronos.Shared.Configuration;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient CreateClient()
        {
            IServerConfiguration configuration = new ServerConfiguration();
            configuration.NodesConfiguration.Add(new NodeConfiguration("8.8.8.8", 7));

            return new KronosClient(new SocketCommunicationService(), configuration);
        }
    }
}