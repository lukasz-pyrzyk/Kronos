using System.IO;
using Google.Protobuf;
using Kronos.Core.Messages;

namespace Kronos.Server
{
    public class SocketProcessor
    {
        public Request ReceiveRequest(Stream stream)
        {
            return Request.Parser.ParseDelimitedFrom(stream);
        }

        public void SendResponse(Stream stream, Response response)
        {
            response.WriteDelimitedTo(stream);
            stream.Flush(); // TODO: async
        }
    }
}
