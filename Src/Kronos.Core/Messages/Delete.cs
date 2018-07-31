namespace Kronos.Core.Messages
{
    public class DeleteRequest
    {
        public string Key { get; set; }

        public static Request New(string key, Auth auth)
        {
            return new Request { Auth = auth, InternalRequest = new DeleteRequest { Key = key }, Type = RequestType.Delete };
        }
    }

    public class DeleteResponse
    {
        public bool Deleted { get; set; }
    }
}
