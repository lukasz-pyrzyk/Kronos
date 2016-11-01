using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct DeleteRequest : IRequest
    {
        [ProtoMember(1)]
        public string Key { get; private set; }

        public RequestType Type => RequestType.Delete;

        public DeleteRequest(string key)
        {
            Key = key;
        }
    }
}
