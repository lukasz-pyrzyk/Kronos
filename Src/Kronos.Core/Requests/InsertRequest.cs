using System;
using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public struct InsertRequest : IRequest
    {
        [ProtoMember(1)]
        public string Key { get; private set; }

        [ProtoMember(2)]
        public byte[] Object { get; private set; }

        [ProtoMember(3)]
        public DateTime ExpiryDate { get; private set; }

        public RequestType Type => RequestType.Insert;

        public InsertRequest(string key, byte[] serializedObject, DateTime expiryDate)
        {
            Key = key;
            Object = serializedObject;
            ExpiryDate = expiryDate;
        }
    }
}
