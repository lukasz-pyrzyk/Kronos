using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct CountRequest : IRequest
    {
        public RequestType Type => RequestType.Count;
    }
}
