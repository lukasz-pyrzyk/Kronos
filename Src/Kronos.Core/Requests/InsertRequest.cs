using System;
using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct InsertRequest
    {
        [ProtoMember(1)]
        public string Key { get; }

        [ProtoMember(2)]
        public byte[] Object { get; }

        [ProtoMember(3)]
        public DateTime ExpiryDate { get; }

        public InsertRequest(string key, byte[] serializedObject, DateTime expiryDate)
        {
            Key = key;
            Object = serializedObject;
            ExpiryDate = expiryDate;
        }
    }
}
