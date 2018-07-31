namespace Kronos.Core.Messages
{
    public class ContainsRequest
    {
        public string Key { get; set; }

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

    public class ContainsResponse
    {
        public bool Contains { get; set; }
    }
}