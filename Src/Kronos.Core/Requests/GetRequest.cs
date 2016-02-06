namespace Kronos.Core.Requests
{
    public class GetRequest : Request
    {
        public override RequestType RequestType { get; set; } = RequestType.GetRequest;
        public string Key { get; set; }

        // used by reflection
        public GetRequest()
        {
        }

        public GetRequest(string key)
        {
            Key = key;
        }
    }
}
