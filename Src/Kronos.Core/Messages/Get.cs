namespace Kronos.Core.Messages
{
    public class GetRequest
    {
        public string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new GetRequest { Key = key }, Type = RequestType.Get };
        }
    }

    public class GetResponse
    {
        public byte[] Data { get; set; }
    }
}
