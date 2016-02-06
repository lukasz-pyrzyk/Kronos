using System.Net;
using Kronos.Client.Transfer;

namespace Kronos.Client
{
    public static class KronosClientFactory
    {
        public static IKronosClient CreateClient(IPAddress host, int port)
        {
            return new KronosClient(new SocketCommunicationService(), new IPEndPoint(host, port));
        }
    }
}