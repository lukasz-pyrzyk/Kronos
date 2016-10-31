using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct ContainsRequest
    {
        [ProtoMember(1)]
        public string Key { get; }

        public ContainsRequest(string key)
        {
            Key = key;
        }
    }
}
