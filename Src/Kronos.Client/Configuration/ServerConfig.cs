using System.Net;
using Kronos.Client.Utils;
using Kronos.Core.Messages;

namespace Kronos.Client.Configuration
{
    public class ServerConfig
    {
        private IPEndPoint _endPoint;
        private Auth _auth;

        public string Domain { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public AuthConfig Credentials { get; set; }

        public Auth Auth => _auth ?? (_auth = PrepareAuth());

        private Auth PrepareAuth()
        {
            if (Credentials != null)
            {
                return Auth.FromCfg(Credentials.Login, Credentials.HashedPassword);
            }
            return Auth.Default();
        }

        public IPEndPoint EndPoint => _endPoint ?? (_endPoint = CreateIpEndPoint());

        private IPEndPoint CreateIpEndPoint()
        {
            IPAddress address;
            if (!string.IsNullOrEmpty(Domain))
            {
                address = EndpointUtils.GetIpAsync(Domain).Result;
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
