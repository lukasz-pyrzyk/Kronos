using System;
using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class DeleteRequest : IRequest
    {
        public ReadOnlyMemory<byte> Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Delete,
                InternalRequest = new DeleteRequest { Key = key.GetMemory() }
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

    public class DeleteResponse : IResponse
    {
        public bool Deleted { get; set; }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Deleted);
        }

        public void Read(ref DeserializationStream stream)
        {
            Deleted = stream.ReadBoolean();
        }
    }
}
