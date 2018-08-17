using System;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct GetRequest : IRequest
    {
        public string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Get,
                InternalRequest = new GetRequest {Key = key}
            };
        }

        public void Write(SerializationStream stream)
        {
            stream.Write(Key);
        }

        public void Read(DeserializationStream stream)
        {
            Key = stream.ReadString();
        }
    }

    public class GetResponse : IResponse
    {
        public ReadOnlyMemory<byte> Data { get; set; }

        public void Write(SerializationStream stream)
        {
            stream.WriteWithPrefixLength(Data.Span);
        }

        public void Read(DeserializationStream stream)
        {
            Data = stream.ReadBytesWithLengthPrefix().ToArray(); // allocation
        }
    }
}
