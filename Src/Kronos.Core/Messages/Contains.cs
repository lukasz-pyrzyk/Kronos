using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class ContainsRequest : IRequest
    {
        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Contains;

        [Index(0)]
        public virtual string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                InternalRequest = new ContainsRequest { Key = key }
            };
        }
    }

    [ZeroFormattable]
    public class ContainsResponse : IResponse
    {
        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Contains;

        [Index(0)]
        public virtual bool Contains { get; set; }
    }
}