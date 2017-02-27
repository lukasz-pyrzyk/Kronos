using System.Runtime.Serialization;

namespace Kronos.Core.Configuration
{
    [DataContract]
    public class AuthConfig
    {
        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
