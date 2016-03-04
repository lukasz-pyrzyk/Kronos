using System;
using Kronos.Core.Model;
using ProtoBuf;

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
    }
}
