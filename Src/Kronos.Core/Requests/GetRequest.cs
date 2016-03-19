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

        public override void ProcessResponse(ISocket socket, IStorage storage)
        {
            byte[] requestedObject = storage.TryGet(Key) ?? SerializationUtils.Serialize(RequestStatusCode.NotFound);
            socket.Send(SerializationUtils.Serialize(requestedObject));
        }

        protected override T ProcessFromClientCode<T>(byte[] responseBytes)
        {
            try
            {
                // if server returned NotFound status code, return null
                RequestStatusCode notFound = SerializationUtils.Deserialize<RequestStatusCode>(responseBytes);
                if (notFound == RequestStatusCode.NotFound)
                {
                    return (T)new object();
                }
            }
            catch (Exception ex)
            {
            }

            return SerializationUtils.Deserialize<T>(responseBytes);
        }
    }
}
