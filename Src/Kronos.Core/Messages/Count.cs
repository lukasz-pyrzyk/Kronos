using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class CountRequest : IRequest
    {
        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Count;

        public static Request New(Auth auth)
        {
            return new Request {Auth = auth, InternalRequest = new CountRequest()};
        }
    }

    [ZeroFormattable]
    public class CountResponse : IResponse
    {
        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Count;

        [Index(0)]
        public virtual int Count { get; set; }
    }
}
