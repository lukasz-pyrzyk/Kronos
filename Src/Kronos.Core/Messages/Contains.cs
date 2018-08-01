using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class ContainsRequest : IRequest
    {
        [IgnoreFormat]
        public virtual byte Id => 2;

        [Index(0)]
        public virtual string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request
            {
                Auth = auth,
                InternalRequest = new ContainsRequest { Key = key },
                Type = RequestType.Contains
            };
        }
    }

    [ZeroFormattable]
    public class ContainsResponse : IResponse
    {
        [IgnoreFormat]
        public virtual byte Id => 2;

        [Index(0)]
        public virtual bool Contains { get; set; }
    }
}