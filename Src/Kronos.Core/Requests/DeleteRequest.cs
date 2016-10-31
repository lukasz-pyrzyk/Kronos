using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct DeleteRequest
    {
        [ProtoMember(1)]
        public string Key { get; }

        public DeleteRequest(string key)
        {
            Key = key;
        }
    }
}
