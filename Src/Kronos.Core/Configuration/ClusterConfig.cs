using System.Runtime.Serialization;

namespace Kronos.Core.Configuration
{
    [DataContract]
    public class ClusterConfig
    {
        [DataMember]
        public ServerConfig[] Servers { get; set; }
    }
}
