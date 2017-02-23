using System;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Kronos.Core.Messages
{
    public partial class InsertRequest
    {
        public static Request New(string key, byte[] package, DateTime? expiryDate)
        {
            return new Request
            {
                InsertRequest = new InsertRequest
                {
                    Key = key,
                    Data = ByteString.CopyFrom(package),
                    Expiry = expiryDate.HasValue ? Timestamp.FromDateTime(expiryDate.Value) : null
                },
                Type = RequestType.Insert
            };
        }
    }
}