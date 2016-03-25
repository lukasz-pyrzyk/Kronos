using System.Net;
using System.Runtime.Serialization;

namespace Kronos.Core.Configuration
{
    [DataContract]
    public class ServerConfig
    {
        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int Port { get; set; }

        public IPEndPoint EndPoint  => new IPEndPoint(IPAddress.Parse(Ip), Port);

        public override string ToString()
        {
            return EndPoint.ToString();
        }
    }
}
