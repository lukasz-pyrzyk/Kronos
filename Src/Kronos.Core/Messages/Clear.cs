using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class ClearRequest : IRequest
    {
        public static Request New(Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new ClearRequest()};
        }

        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Clear;
    }

    [ZeroFormattable]
    public class ClearResponse : IResponse
    {
        [Index(0)]
        public virtual int Deleted { get; set; }

        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Clear;
    }
}

