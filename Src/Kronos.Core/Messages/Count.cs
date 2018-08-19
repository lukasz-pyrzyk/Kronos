using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class CountRequest : IRequest
    {
        public static Request New(Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Count,
                InternalRequest = new CountRequest()
            };
        }

        public void Write(ref SerializationStream stream)
        {
        }

        public void Read(ref DeserializationStream stream)
        {
        }
    }

    public class CountResponse : IResponse
    {
        public int Count { get; set; }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Count);
        }

        public void Read(ref DeserializationStream stream)
        {
            Count = stream.ReadInt();
        }
    }
}
