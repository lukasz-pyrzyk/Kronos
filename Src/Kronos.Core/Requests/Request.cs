using ProtoBuf;

namespace Kronos.Core.Requests
{
    /// <summary>
    /// Base class for any request to the server
    /// </summary>
    public abstract class Request
    {
        public abstract RequestType RequestType { get; set; }
    }
}
