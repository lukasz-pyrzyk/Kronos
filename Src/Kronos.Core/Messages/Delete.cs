using Kronos.Core.Serialization;

namespace Kronos.Core.Messages
{
    public struct DeleteRequest : IRequest
    {
        public string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                Type = RequestType.Delete,
                InternalRequest = new DeleteRequest {Key = key}
            };
        }

        public void Write(ref SerializationStream stream)
        {
            stream.Write(Key);
        }

        public void Read(ref DeserializationStream stream)
        {
            Key = stream.ReadString();
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
