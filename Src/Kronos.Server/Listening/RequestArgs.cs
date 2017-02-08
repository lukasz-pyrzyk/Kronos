using System.Net.Sockets;
using Kronos.Core.Requests;

namespace Kronos.Server.Listening
{
    public struct RequestArg
    {
        public void Assign(RequestType type, byte[] request, int received, Socket client)
        {
            Type = type;
            Bytes = request;
            Count = received;
            Client = client;
        }

        public RequestType Type { get; private set; }
        public byte[] Bytes { get; private set; }
        public int Count { get; private set; }
        public Socket Client { get; private set; }
    }
}
