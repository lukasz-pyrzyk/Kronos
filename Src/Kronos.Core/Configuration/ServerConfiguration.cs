using System.Collections.Generic;
using System.Linq;
using Kronos.Core.Model;

namespace Kronos.Core.Configuration
{
    public class ServerConfiguration : IServerConfiguration
    {
        public ICollection<NodeConfiguration> NodesConfiguration { get; set; } = new List<NodeConfiguration>();

        public NodeConfiguration GetNodeForStream(CachedObject objectToCache)
        {
            return NodesConfiguration.FirstOrDefault();
        }
    }
}
