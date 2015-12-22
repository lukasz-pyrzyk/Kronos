using System.Collections.Generic;
using System.IO;

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
        /// <param name="stream">Stream to insert into server</param>
        /// <returns></returns>
        NodeConfiguration GetNodeForStream(Stream stream);
    }
}
