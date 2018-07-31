namespace Kronos.Core.Messages
{
    public class Request
    {
        public Auth Auth { get; set; }
        public RequestType Type { get; set; }
        public object InternalRequest { get; set; }
    }

    public class Response
    {
        public string Exception { get; set; }

        public bool Success => string.IsNullOrEmpty(Exception);
        public object InternalResponse { get; set; }
    }
}
