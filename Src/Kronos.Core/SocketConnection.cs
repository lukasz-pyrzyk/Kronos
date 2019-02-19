using System.IO;
using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Core.Messages;

namespace Kronos.Core
{
    public class SocketConnection
    {
        public async Task Send(Request request, Stream stream)
        {
            request.WriteDelimitedTo(stream);
            await stream.FlushAsync();
        }

        public async Task Send(Response response, Stream stream)
        {
            response.WriteDelimitedTo(stream);
            await stream.FlushAsync();
        }

        public Request ReceiveRequest(Stream stream)
        {
            return Request.Parser.ParseDelimitedFrom(stream);
        }

        public Response ReceiveResponse(Stream stream)
        {
            return Response.Parser.ParseDelimitedFrom(stream);
        }
    }
}
