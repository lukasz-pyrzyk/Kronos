using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct ClearRequest : IRequest
    {
        public RequestType Type => RequestType.Clear;
    }
}
