using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class ContainsRequest : Request
    {
        public override RequestType RequestType => RequestType.Contains;

        [ProtoMember(1)]
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
            bool contains = storage.Contains(Key);

            socket.Send(SerializationUtils.SerializeToStreamWithLength(contains));
        }
    }
}
