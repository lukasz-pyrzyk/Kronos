using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class ContainsRequest : Request
    {
        public string Key { get; set; }

        public ContainsRequest()
        {
        }

        public ContainsRequest(string key)
        {
            Key = key;
        }

        public override void ProcessAndSendResponse(ISocket socket, IStorage storage)
        {

        }
    }
}
