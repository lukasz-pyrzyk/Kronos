using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class GetRequest : Request
    {
        public override RequestType RequestType { get; set; } = RequestType.GetRequest;

        [ProtoMember(1)]
        public string Key { get; set; }

        // used by reflection
        public GetRequest()
        {
        }

        public GetRequest(string key)
        {
            Key = key;
        }

        public override void ProcessRequest(ISocket socket, IStorage storage)
        {
            throw new System.NotImplementedException();
        }
    }
}
