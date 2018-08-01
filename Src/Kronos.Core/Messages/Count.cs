using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class CountRequest : IRequest
    {
        [IgnoreFormat]
        public virtual byte Id => 3;

        public static Request New(Auth auth)
        {
            return new Request {Auth = auth, InternalRequest = new CountRequest(), Type = RequestType.Count};
        }
    }

    [ZeroFormattable]
    public class CountResponse : IResponse
    {
        [IgnoreFormat]
        public virtual byte Id => 3;

        [Index(0)]
        public virtual int Count { get; set; }
    }
}
