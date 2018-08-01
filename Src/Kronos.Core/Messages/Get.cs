using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class GetRequest : IRequest
    {
        [IgnoreFormat]
        public virtual byte Id => 5;

        [Index(0)]
        public virtual string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new GetRequest { Key = key }, Type = RequestType.Get };
        }
    }

    [ZeroFormattable]
    public class GetResponse : IResponse
    {
        [IgnoreFormat]
        public virtual byte Id => 5;

        [Index(0)]
        public virtual byte[] Data { get; set; }
    }
}
