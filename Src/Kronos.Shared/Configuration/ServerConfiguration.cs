using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kronos.Shared.Configuration
{
    public class ServerConfiguration : IServerConfiguration
    {
        public ICollection<NodeConfiguration> NodesConfiguration { get; set; } = new List<NodeConfiguration>();

        public NodeConfiguration GetNodeForStream(byte[] package)
        {
            return NodesConfiguration.FirstOrDefault();
        }
    }
}
