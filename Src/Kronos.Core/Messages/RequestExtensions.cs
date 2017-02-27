using System;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Kronos.Core.Configuration;

namespace Kronos.Core.Messages
{
    public partial class InsertRequest
    {
        public static Request New(string key, byte[] package, DateTime? expiryDate, AuthConfig auth)
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

    public partial class GetRequest
    {
        public static Request New(string key)
        {
            return new Request
            {
                GetRequest = new GetRequest
                {
                    Key = key
                },
                Type = RequestType.Get
            };
        }
    }

    public partial class DeleteRequest
    {
        public static Request New(string key)
        {
            return new Request
            {
                DeleteRequest = new DeleteRequest
                {
                    Key = key
                },
                Type = RequestType.Delete
            };
        }
    }

    public partial class CountRequest
    {
        public static Request New()
        {
            return new Request
            {
                CountRequest = new CountRequest(),
                Type = RequestType.Count
            };
        }
    }

    public partial class ContainsRequest
    {
        public static Request New(string key)
        {
            return new Request
            {
                ContainsRequest = new ContainsRequest
                {
                    Key = key
                },
                Type = RequestType.Contains
            };
        }
    }

    public partial class ClearRequest
    {
        public static Request New()
        {
            return new Request
            {
                ClearRequest = new ClearRequest(),
                Type = RequestType.Clear
            };
        }
    }
}