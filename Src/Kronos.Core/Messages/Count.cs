namespace Kronos.Core.Messages
{
    public class CountRequest
    {
        public static Request New(Auth auth)
        {
            return new Request {Auth = auth, InternalRequest = new CountRequest(), Type = RequestType.Count};
        }
    }

    public class CountResponse
    {
        public int Count { get; set; }
    }
}
