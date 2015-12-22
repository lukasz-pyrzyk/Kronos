using System.Net;
using System.Text;

namespace Kronos.Shared.Network.Requests
{
    /// <summary>
    /// Base class for any request to the server
    /// </summary>
    public class Request
    {
        public Request(string host, int port)
        {
            Host = host;
            Port = port;
            Endpoint = new IPEndPoint(IPAddress.Parse(Host), Port);
        }

        /// <summary>
        /// Server host address (IP or DNS name)
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Opened port in server
        /// </summary>
        public int Port { get; }

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
