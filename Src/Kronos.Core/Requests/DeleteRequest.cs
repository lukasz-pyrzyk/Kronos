using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class DeleteRequest : Request
    {
        public override RequestType RequestType { get; set; } = RequestType.Delete;

        [ProtoMember(1)]
        public string Key { get; set; }

        // used by reflection
        public DeleteRequest()
        {
        }

        public DeleteRequest(string key)
        {
            Key = key;
        }

        public override void ProcessAndSendResponse(ISocket socket, IStorage storage)
        {
            bool deleted = storage.TryRemove(Key);
            RequestStatusCode code = deleted ? RequestStatusCode.Deleted : RequestStatusCode.NotFound;

            socket.Send(SerializationUtils.Serialize(code));
        }
    }
}
