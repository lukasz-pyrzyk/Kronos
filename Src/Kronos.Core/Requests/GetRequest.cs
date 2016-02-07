using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class GetRequest : Request
    {
        [ProtoMember(1, IsRequired = true)]
        public override RequestType RequestType { get; set; } = RequestType.GetRequest;

        [ProtoMember(2)]
        public string Key { get; set; }

        // used by reflection
        public GetRequest()
        {
        }

        public GetRequest(string key)
        {
            Key = key;
        }
    }
}
