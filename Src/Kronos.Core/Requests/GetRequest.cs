using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct GetRequest : IRequest
    {
        [ProtoMember(1)]
        public string Key { get; private set; }

        public RequestType Type => RequestType.Get;

        public GetRequest(string key)
        {
            Key = key;
        }
    }
}
