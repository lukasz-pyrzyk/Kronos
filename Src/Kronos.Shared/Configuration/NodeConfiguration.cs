namespace Kronos.Shared.Configuration
{
    /// <summary>
    /// Represents Kronos node configuration
    /// </summary>
    public class NodeConfiguration
    {
        /// <summary>
        /// Host of node
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Opened port
        /// </summary>
        public int Port { get; set; }
    }
}
