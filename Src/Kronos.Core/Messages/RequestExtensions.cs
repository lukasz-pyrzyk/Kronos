using System;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Kronos.Core.Configuration;

namespace Kronos.Core.Messages
{
    public partial class Auth
    {
        public static Auth FromCfg(string login, byte[] password)
        {
            return new Auth
            {
                Login = login,
                HashedPassword = ByteString.CopyFrom(password)
            };
        }

        public static Auth Default()
        {
            return FromCfg(DefaultSettings.Login, DefaultSettings.HashedPassword);
        }

        public bool Authorize(Auth auth)
        {
            return auth.Login == Login && auth.HashedPassword == HashedPassword;
        }
    }

    public partial class InsertRequest
    {
        public static Request New(string key, byte[] package, DateTime? expiryDate, Auth auth)
        {
            return new Request
            {
                InsertRequest = new InsertRequest
                {
                    Key = key,
                    Data = ByteString.CopyFrom(package),
                    Expiry = expiryDate.HasValue ? Timestamp.FromDateTime(expiryDate.Value) : null
                },
                Type = RequestType.Insert,
                Auth = auth
            };
        }
    }

    public partial class GetRequest
    {
        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                GetRequest = new GetRequest
                {
                    Key = key
                },
                Type = RequestType.Get,
                Auth = auth
            };
        }
    }

    public partial class DeleteRequest
    {
        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                DeleteRequest = new DeleteRequest
                {
                    Key = key
                },
                Type = RequestType.Delete,
                Auth = auth
            };
        }
    }

    public partial class CountRequest
    {
        public static Request New(Auth auth)
        {
            return new Request
            {
                CountRequest = new CountRequest(),
                Type = RequestType.Count,
                Auth = auth
            };
        }
    }

    public partial class ContainsRequest
    {
        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                ContainsRequest = new ContainsRequest
                {
                    Key = key
                },
                Type = RequestType.Contains,
                Auth = auth
            };
        }
    }

    public partial class ClearRequest
    {
        public static Request New(Auth auth)
        {
            return new Request
            {
                ClearRequest = new ClearRequest(),
                Type = RequestType.Clear,
                Auth = auth
            };
        }
    }

    public partial class StatsRequest
    {
        public static Request New(Auth auth)
        {
            return new Request
            {
                StatsRequest = new StatsRequest(),
                Type = RequestType.Stats,
                Auth = auth
            };
        }
    }

    public partial class Response
    {
        public bool Success => string.IsNullOrEmpty(Exception);
    }
}