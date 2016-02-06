using System.Net;
using System.Text;

namespace Kronos.Core.Configuration
{
    /// <summary>
    /// Represents Kronos node configuration
    /// </summary>
    public class NodeConfiguration
    {
        public NodeConfiguration(IPAddress host, int port)
        {
            Host = host;
            Port = port;
            Endpoint = new IPEndPoint(host, Port);
        }

        /// <summary>
        /// Host of node
        /// </summary>
        public IPAddress Host { get; set; }

        /// <summary>
        /// Opened port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// IP endpoint
        /// </summary>
        public IPEndPoint Endpoint { get; }

        public override string ToString()
        {
            return Endpoint.ToString();
        }
    }
}
