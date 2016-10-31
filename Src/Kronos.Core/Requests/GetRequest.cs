using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct GetRequest
    {
        [ProtoMember(1)]
        public string Key { get; }

        public GetRequest(string key)
        {
            Key = key;
        }
    }
}
