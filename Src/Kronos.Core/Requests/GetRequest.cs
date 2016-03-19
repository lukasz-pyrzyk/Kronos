using System;
using Kronos.Core.Communication;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
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

        public byte[] Execute(IClientServerConnection service)
        {
            byte[] response = service.SendToServer(this);

            try
            {
                // if server returned NotFound status code, return null
                RequestStatusCode notFound = SerializationUtils.Deserialize<RequestStatusCode>(response);
                if (notFound == RequestStatusCode.NotFound)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
            }

            return SerializationUtils.Deserialize<byte[]>(response);
        }

        public override void ProcessRequest(ISocket socket, IStorage storage)
        {
            byte[] requestedObject = storage.TryGet(Key) ?? SerializationUtils.Serialize(RequestStatusCode.NotFound);
            socket.Send(SerializationUtils.Serialize(requestedObject));
        }
    }
}
