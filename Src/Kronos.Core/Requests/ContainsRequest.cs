using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct ContainsRequest : IRequest
    {
        [ProtoMember(1)]
        public string Key { get; private set; }

        public RequestType Type => RequestType.Contains;

        public ContainsRequest(string key)
        {
            Key = key;
        }
    }
}
