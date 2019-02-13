using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;
using Kronos.Core.Networking;

namespace Kronos.Client
{
    public class Connection : IConnection
    {
        public async Task<Response> SendAsync(Request request, ServerConfig server)
        {
            Socket socket = null;
            Response response;
            try
            {
                Trace.WriteLine("Connecting to the server socket");
                socket = new Socket(SocketType.Stream, ProtocolType.IP);
                await socket.ConnectAsync(server.EndPoint).ConfigureAwait(false);

                Trace.WriteLine("Sending request");
                await SendAsync(request, socket).ConfigureAwait(false);

                Trace.WriteLine("Waiting for response");
                response = await ReceiveAndDeserializeAsync(socket).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new KronosCommunicationException($"Connection to the {server.EndPoint} has been refused", ex);
            }
            finally
            {
                socket?.Dispose();
            }

            return response;
        }

        private async Task SendAsync(Request request, Socket server)
        {
            using (var stream = new NetworkStream(server, FileAccess.Write, false))
            {
                request.WriteDelimitedTo(stream);
                await stream.FlushAsync();
            }
        }

        private async Task<Response> ReceiveAndDeserializeAsync(Socket socket)
        {
            using (var stream = new NetworkStream(socket, FileAccess.Read, false))
            {
                return Response.Parser.ParseDelimitedFrom(stream);
            }
        }
    }
}