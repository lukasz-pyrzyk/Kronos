using System;
using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class InsertRequest : IRequest
    {
        [IgnoreFormat]
        public virtual byte Id => 6;

        [Index(0)]
        public virtual string Key { get; set; }

        [Index(1)]
        public virtual DateTime? Expiry { get; set; }

        [Index(2)]
        public virtual byte[] Data { get; set; }

        public static Request New(string key, byte[] data, DateTime? expiry, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                InternalRequest = new InsertRequest
                {
                    Data = data,
                    Expiry = expiry,
                    Key = key
                },
                Type = RequestType.Insert
            };
        }
    }

    [ZeroFormattable]
    public class InsertResponse : IResponse
    {
        [IgnoreFormat]
        public virtual byte Id => 6;

        [Index(0)]
        public virtual bool Added { get; set; }
    }
}
