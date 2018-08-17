using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct ClearRequest : IRequest
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

        public void Write(SerializationStream stream)
        {
        }

        public void Read(DeserializationStream stream)
        {
        }
    }

    public class ClearResponse : IResponse
    {
        public int Deleted { get; set; }

        public void Write(SerializationStream stream)
        {
            stream.Write(Deleted);
        }

        public void Read(DeserializationStream stream)
        {
            Deleted = stream.ReadInt();
        }
    }
}

