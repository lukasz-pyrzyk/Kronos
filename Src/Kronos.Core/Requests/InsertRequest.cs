using System;
using Kronos.Core.Model;
using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class InsertRequest : Request
    {
        [ProtoMember(1, IsRequired = true)]
        public override RequestType RequestType { get; set; } = RequestType.InsertRequest;

        [ProtoMember(2)]
        public string Key { get; set; }

        [ProtoMember(3)]
        public byte[] Object { get; set; }

        [ProtoMember(4)]
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
