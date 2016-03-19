using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    /// <summary>
    /// Base class for any request to the server
    /// </summary>
    [ProtoContract]
    [ProtoInclude(500, typeof(InsertRequest))]
    [ProtoInclude(1000, typeof(GetRequest))]
    public abstract class Request
    {
        public virtual RequestType RequestType { get; set; }

        public abstract void ProcessRequest(ISocket socket, IStorage storage);
    }
}
