using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using BufferedStream = Kronos.Core.Pooling.BufferedStream;

namespace Kronos.Client
{
    public class Connection : IConnection
    {
        private readonly byte[] _sizeBytes = new byte[sizeof(int)];
        private readonly BufferedStream _stream = new BufferedStream();

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
                if (!_stream.IsClean) _stream.Clean();

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
            await SocketUtils.ReceiveAllAsync(socket, _sizeBytes, _sizeBytes.Length).ConfigureAwait(false);
            var packageSize = BitConverter.ToInt32(_sizeBytes, 0);
            Array.Clear(_sizeBytes, 0, _sizeBytes.Length);

            var requestBytes = ArrayPool<byte>.Shared.Rent(packageSize);
            try
            {
                await SocketUtils.ReceiveAllAsync(socket, requestBytes, packageSize).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(requestBytes);
            }

            var response = Response.Parser.ParseFrom(new CodedInputStream(requestBytes, 0, packageSize));

            return response;
        }
    }
}