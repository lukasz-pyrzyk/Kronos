using ProtoBuf;

namespace Kronos.Core.Requests
{
    /// <summary>
    /// Base class for any request to the server
    /// </summary>
    [ProtoContract]
    [ProtoInclude(500, typeof(InsertRequest))]
    [ProtoInclude(100, typeof(GetRequest))]
    public class Request
    {
        public virtual RequestType RequestType { get; set; }
    }
}
