using System;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;

namespace Kronos.Core.Messages
{
    public partial class Auth
    {
        public const string DefaultLogin = "user";
        public const string DefaultPassword = "password";

        public static Auth FromCfg(AuthConfig cfg)
        {
            return new Auth
            {
                Login = cfg.Login,
                HashedPassword = ByteString.CopyFrom(cfg.HashedPassword)
            };
        }

        public static Auth Default()
        {
            return FromCfg(new AuthConfig
            {
                Login = DefaultLogin,
                Password = DefaultPassword
            });
        }

        public bool Authorize(Auth auth)
        {
            return auth.Login == Login && auth.HashedPassword == HashedPassword;
        }
    }

    public partial class InsertRequest
    {
        public static Request New(string key, byte[] data, DateTime? expiryDate, Auth auth)
        {
            int maxSize = Settings.MaxRequestSize;
            if (data.Length > maxSize)
            {
                throw new KronosException($"Content size exceeded. Maximum value is {maxSize} bytes");
            }

            return new Request
            {
                InsertRequest = new InsertRequest
                {
                    Key = key,
                    Data = ByteString.CopyFrom(data),
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

    public partial class Response
    {
        public bool Success => string.IsNullOrEmpty(Exception);
    }
}