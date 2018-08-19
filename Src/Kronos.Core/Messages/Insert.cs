using System;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class InsertRequest : IRequest
    {
        public ReadOnlyMemory<byte> Key { get; set; }

        public DateTimeOffset? Expiry { get; set; }

        public ReadOnlyMemory<byte> Data { get; set; }

        public static Request New(string key, byte[] data, DateTimeOffset? expiry, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Insert,
                InternalRequest = new InsertRequest
                {
                    Data = data,
                    Expiry = expiry,
                    Key = key.GetMemory()
                }
            };
        }

        public void Write(ref SerializationStream stream)
        {
            stream.WriteWithPrefixLength(Key.Span);
            stream.WriteWithPrefixLength(Data.Span);
            stream.Write(Expiry);
        }

        public void Read(ref DeserializationStream stream)
        {
            Key = stream.ReadMemory();
            Data = stream.ReadMemory();
            Expiry = stream.ReadDateTimeOffset();
        }
    }

    public class InsertResponse : IResponse
    {
        public bool Added { get; set; }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Added);
        }

        public void Read(ref DeserializationStream stream)
        {
            Added = stream.ReadBoolean();
        }
    }
}
