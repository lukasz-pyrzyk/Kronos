namespace Kronos.Core.Messages
{
    public class StatsRequest
    {
        public static Request New(Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new StatsRequest(), Type = RequestType.Stats };
        }
    }

    public class StatsResponse
    {
        public int MemoryUsed { get; set; }
        public long Elements { get; set; }
    }
}
