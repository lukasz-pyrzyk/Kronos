using System.Net;
using System.Runtime.Serialization;
using Kronos.Core.Messages;
using Kronos.Core.Networking;

namespace Kronos.Core.Configuration
{
    [DataContract]
    public class ServerConfig
    {
        private IPEndPoint _endPoint;
        private Auth _auth;

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public AuthConfig AuthConfig { get; set; }

        public Auth Auth => _auth ?? (_auth = PrepareAuth());

        private Auth PrepareAuth()
        {
            if (AuthConfig != null)
            {
                return Auth.FromCfg(AuthConfig);
            }
            return Auth.Default();
        }

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
