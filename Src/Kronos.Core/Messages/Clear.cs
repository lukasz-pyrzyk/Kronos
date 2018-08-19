using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public class ClearRequest : IRequest
    {
        public static Request New(Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Clear,
                InternalRequest = new ClearRequest()
            };
        }

        public void Write(ref SerializationStream stream)
        {
        }

        public void Read(ref DeserializationStream stream)
        {
        }
    }

    public class ClearResponse : IResponse
    {
        public int Deleted { get; set; }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Deleted);
        }

        public void Read(ref DeserializationStream stream)
        {
            Deleted = stream.ReadInt();
        }
    }
}

