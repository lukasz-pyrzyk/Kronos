using System.Collections.Generic;
using Kronos.Core.Model;

namespace Kronos.Core.Configuration
{
    /// <summary>
    /// Represents a server configuration
    /// </summary>
    public interface IServerConfiguration
    {
        /// <summary>
        /// Collection of nodes configurations
        /// </summary>
        ICollection<NodeConfiguration> NodesConfiguration { get; set; }

        /// <summary>
        /// Select node for stream
        /// </summary>
        /// <param name="objectToCache">Package to insert into server</param>
        /// <returns></returns>
        NodeConfiguration GetNodeForStream(CachedObject objectToCache);
    }
}
