using System;

namespace Kronos.Core.Messages
{
    public class InsertRequest
    {
        public string Key { get; set; }
        public DateTime? Expiry { get; set; }
        public byte[] Data { get; set; }

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

    public class InsertResponse
    {
        public bool Added { get; set; }
    }
}
