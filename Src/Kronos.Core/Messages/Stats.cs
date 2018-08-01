using ZeroFormatter;

namespace Kronos.Core.Messages
{
    [ZeroFormattable]
    public class StatsRequest : IRequest
    {
        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Stats;

        public static Request New(Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new StatsRequest()};
        }
    }

    [ZeroFormattable]
    public class StatsResponse : IResponse
    {
        [IgnoreFormat]
        public virtual RequestType Type => RequestType.Stats;

        [Index(0)]
        public virtual int MemoryUsed { get; set; }

        [Index(1)]
        public virtual long Elements { get; set; }
    }
}
