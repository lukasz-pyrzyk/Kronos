namespace Kronos.Core.Messages
{
    public class ClearRequest
    {
        public static Request New(Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new ClearRequest(), Type = RequestType.Clear };
        }
    }

    public class ClearResponse
    {
        public int Deleted { get; set; }
    }
}

