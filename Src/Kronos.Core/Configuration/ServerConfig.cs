using System.Net;
using System.Runtime.Serialization;
using Kronos.Core.Networking;

namespace Kronos.Core.Configuration
{
    [DataContract]
    public class ServerConfig
    {
        private IPEndPoint _endPoint;

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int Port { get; set; }

        public IPEndPoint EndPoint => _endPoint ?? (_endPoint = CreateIPEndPoint());

        private IPEndPoint CreateIPEndPoint()
        {
            IPAddress address;
            if (!string.IsNullOrEmpty(Domain))
            {
                address = EndpointUtils.GetIPAsync(Domain).Result;
                Ip = address.ToString();
            }
            else
            {
                address = IPAddress.Parse(Ip);
            }

            return new IPEndPoint(address, Port);
        }

        public override string ToString()
        {
            return EndPoint.ToString();
        }
    }
}
