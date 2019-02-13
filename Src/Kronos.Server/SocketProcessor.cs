using System.IO;
using System.Net.Sockets;
using Google.Protobuf;
using Kronos.Core.Messages;

namespace Kronos.Server
{
    public class SocketProcessor : ISocketProcessor
    {
        public Request ReceiveRequest(Socket client)
        {
            using (var stream = new NetworkStream(client, FileAccess.Read, false))
            {
                return Request.Parser.ParseDelimitedFrom(stream);
            }
        }

        public void SendResponse(Socket client, Response response)
        {
            using (var stream = new NetworkStream(client, FileAccess.Write, false))
            {
                response.WriteDelimitedTo(stream);
                stream.Flush(); // TODO: async
            }
        }
    }
}
