using System.Collections.Generic;
using System.IO;
using Kronos.Shared.Network.Model;

namespace Kronos.Shared.Configuration
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
