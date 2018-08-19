using System;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class ContainsRequest : IRequest
    {
        public ReadOnlyMemory<byte> Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Contains,
                InternalRequest = new ContainsRequest { Key = key.GetMemory() }
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

    public class ContainsResponse : IResponse
    {
        public static readonly ContainsResponse CachedExists = new ContainsResponse() { Contains = true };
        public static readonly ContainsResponse CachedMissing = new ContainsResponse() { Contains = false };

        public bool Contains { get; private set; }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Contains);
        }

        public void Read(ref DeserializationStream stream)
        {
            Contains = stream.ReadBoolean();
        }
    }
}