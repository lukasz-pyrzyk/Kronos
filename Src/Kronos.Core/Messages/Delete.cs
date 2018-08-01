using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class DeleteRequest : IRequest
    {
        [IgnoreFormat]
        public virtual byte Id => 4;

        [Index(0)]
        public virtual string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new DeleteRequest { Key = key }, Type = RequestType.Delete };
        }
    }

    [ZeroFormattable]
    public class DeleteResponse : IResponse
    {
        [IgnoreFormat]
        public virtual byte Id => 4;

        [Index(0)]
        public virtual bool Deleted { get; set; }
    }
}
