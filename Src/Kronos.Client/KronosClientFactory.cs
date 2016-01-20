using Kronos.Client.Transfer;
using Kronos.Shared.Configuration;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient CreateClient()
        {
            IServerConfiguration configuration = new ServerConfiguration();
            configuration.NodesConfiguration.Add(new NodeConfiguration("40.117.236.187", 5000));

            return new KronosClient(new SocketCommunicationService(), configuration);
        }
    }
}