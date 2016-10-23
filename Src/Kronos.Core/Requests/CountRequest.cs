using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class CountRequest : Request
    {
        public override RequestType RequestType => RequestType.Count;

        public override void ProcessAndSendResponse(ISocket socket, IStorage storage)
        {
            int count = storage.Count;

            socket.Send(SerializationUtils.SerializeToStreamWithLength(count));
        }
    }
}
