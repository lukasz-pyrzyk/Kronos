using Kronos.Client.Core.Server;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient CreateClient()
        {
            return new KronosClient(new KronosCommunicationServiceSocket());
        }
    }
}