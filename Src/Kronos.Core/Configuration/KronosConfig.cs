using System.Runtime.Serialization;

namespace Kronos.Core.Configuration
{
    [DataContract]
    public class KronosConfig
    {
        [DataMember]
        public ClusterConfig ClusterConfig { get; set; }
    }
}
