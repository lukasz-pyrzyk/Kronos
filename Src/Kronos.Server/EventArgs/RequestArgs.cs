using Kronos.Core.Requests;

namespace Kronos.Server.EventArgs
{
    public class RequestArgs : System.EventArgs
    {
        public RequestArgs(RequestType type, byte[] request, int received)
        {
            Type = type;
            Request = request;
            Received = received;
        }

        public RequestType Type { get; }
        public int Received { get; }
        public byte[] Request { get; }
    }
}
