using System;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class GetRequest : IRequest
    {
        public ReadOnlyMemory<byte> Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Get,
                InternalRequest = new GetRequest { Key = key.GetMemory() }
            };
        }

        public void Write(ref SerializationStream stream)
        {
            stream.WriteWithPrefixLength(Key.Span);
        }

        public void Read(ref DeserializationStream stream)
        {
            Key = stream.ReadMemory();
        }
    }

    public class GetResponse : IResponse
    {
        public ReadOnlyMemory<byte> Data { get; set; }

        public void Write(ref SerializationStream stream)
        {
            stream.WriteWithPrefixLength(Data.Span);
        }

        public void Read(ref DeserializationStream stream)
        {
            Data = stream.ReadMemoryWithLengthPrefix();
        }
    }
}
