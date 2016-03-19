using System;
using Kronos.Core.Communication;
using Kronos.Core.Model;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class InsertRequest : Request
    {
        public override RequestType RequestType { get; set; } = RequestType.InsertRequest;

        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public byte[] Object { get; set; }

        [ProtoMember(3)]
        public DateTime ExpiryDate { get; set; }

        // used by reflection
        public InsertRequest()
        {
        }

        public InsertRequest(string key, byte[] serializedObject, DateTime expiryDate)
        {
            Key = key;
            Object = serializedObject;
            ExpiryDate = expiryDate;
        }

        public RequestStatusCode Execute(IClientServerConnection service, InsertRequest request)
        {
            byte[] response = service.SendToServer(request);
            RequestStatusCode statusCode = SerializationUtils.Deserialize<RequestStatusCode>(response);

            return statusCode;
        }

        public override void ProcessRequest(ISocket socket, IStorage storage)
        {
            storage.AddOrUpdate(Key, Object);
            socket.Send(SerializationUtils.Serialize(RequestStatusCode.Ok));
        }
    }
}
