using System.Net;

namespace Kronos.Shared.Configuration
{
    /// <summary>
    /// Represents Kronos node configuration
    /// </summary>
    public class NodeConfiguration
    {
        public NodeConfiguration(string host, int port)
        {
            Host = host;
            Port = port;
            Endpoint = new IPEndPoint(IPAddress.Parse(Host), Port);
        }

        /// <summary>
        /// Host of node
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Opened port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// IP endpoint
        /// </summary>
        public IPEndPoint Endpoint { get; private set; }

        public override string ToString()
        {
            return Endpoint.ToString();
        }
    }
}
