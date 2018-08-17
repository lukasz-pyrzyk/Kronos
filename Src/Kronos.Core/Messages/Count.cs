using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct CountRequest : IRequest
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

        public void Write(SerializationStream stream)
        {
        }

        public void Read(DeserializationStream stream)
        {
        }
    }

    public class CountResponse : IResponse
    {
        public int Count { get; set; }

        public void Write(SerializationStream stream)
        {
            stream.Write(Count);
        }

        public void Read(DeserializationStream stream)
        {
            Count = stream.ReadInt();
        }
    }
}
