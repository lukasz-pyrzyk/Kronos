using System.IO;
using System.Net.Sockets;
using Google.Protobuf;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using BufferedStream = Kronos.Core.Pooling.BufferedStream;

namespace Kronos.Server
{
    public class SocketProcessor : ISocketProcessor
    {
        private readonly BufferedStream _stream = new BufferedStream();

        public Request ReceiveRequest(Socket client)
        {
            using (var stream = new NetworkStream(client, FileAccess.Read, false))
            {
                var request = Request.Parser.ParseDelimitedFrom(stream);

                return request;
            }
        }

        public void SendResponse(Socket client, Response response)
        {
            response.WriteTo(_stream);

            try
            {
                SocketUtils.SendAll(client, _stream.RawBytes, (int)_stream.Length);
            }
            finally
            {
                _stream.Clean();
            }
        }
    }
}
