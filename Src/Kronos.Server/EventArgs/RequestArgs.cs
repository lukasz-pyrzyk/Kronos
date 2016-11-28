using System.Net.Sockets;
using Kronos.Core.Requests;

namespace Kronos.Server.EventArgs
{
    public class RequestArgs : System.EventArgs
    {
        public RequestArgs(RequestType type, byte[] request, int received, Socket client)
        {
            Type = type;
            Request = request;
            Received = received;
            Client = client;
        }

        public RequestType Type { get; }
        public int Received { get; }
        public Socket Client { get; }
        public byte[] Request { get; }
    }
}
