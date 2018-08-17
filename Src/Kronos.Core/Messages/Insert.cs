using System;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct InsertRequest : IRequest
    {
        public string Key { get; set; }

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
                    Key = key
                }
            };
        }

        public void Write(SerializationStream stream)
        {
            stream.Write(Key);
            stream.WriteWithPrefixLength(Data.Span);
            stream.Write(Expiry);
        }

        public void Read(DeserializationStream stream)
        {
            Key = stream.ReadString();
            Data = stream.ReadMemory();
            Expiry = stream.ReadDateTimeOffset();
        }
    }

    public class InsertResponse : IResponse
    {
        public bool Added { get; set; }

        public void Write(SerializationStream stream)
        {
            stream.Write(Added);
        }

        public void Read(DeserializationStream stream)
        {
            Added = stream.ReadBoolean();
        }
    }
}
