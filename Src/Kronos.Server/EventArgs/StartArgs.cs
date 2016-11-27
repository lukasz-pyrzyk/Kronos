using System.Net;

namespace Kronos.Server.EventArgs
{
    public class StartArgs : System.EventArgs
    {
        public StartArgs(EndPoint localEndpoint)
        {
            LocalEndpoint = localEndpoint;
        }

        public EndPoint LocalEndpoint { get; }
    }
}
