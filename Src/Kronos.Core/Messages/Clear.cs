using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class ClearRequest : IRequest
    {
        [IgnoreFormat]
        public virtual byte Id => 1;

        public static Request New(Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new ClearRequest(), Type = RequestType.Clear };
        }
    }

    [ZeroFormattable]
    public class ClearResponse : IResponse
    {
        [IgnoreFormat]
        public virtual byte Id => 1;

        [Index(0)]
        public virtual int Deleted { get; set; }
    }
}

