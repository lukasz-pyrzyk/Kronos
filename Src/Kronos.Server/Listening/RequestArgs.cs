using System.Net.Sockets;

namespace Kronos.Server.Listening
{
    public struct RequestArg
    {
        public RequestArg(Request request, Socket client)
        {
            Request = request;
            Client = client;
        }

        public Request Request { get; private set; }

        public Socket Client { get; private set; }
    }
}
