using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using Kronos.Core.Pooling;

namespace Kronos.Client
{
    public class Connection : IConnection
    {
        private readonly byte[] _sizeBytes = new byte[sizeof(int)];
        private readonly BufferedStream _stream = new BufferedStream();

        public async Task<Response> SendAsync(Request request, ServerConfig server)
        {
            Socket socket = null;
            Response response = null;
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
            request.WriteTo(_stream);

            try
            {
                await SocketUtils.SendAllAsync(server, _stream.RawBytes, (int) _stream.Length).ConfigureAwait(false);
            }
            finally
            {
                _stream.Clean();
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

        private static IEnumerable<TimeSpan> CreateExponentialBackoff(int retryCount)
        {
            for (var i = 1; i <= retryCount; i++) yield return TimeSpan.FromSeconds(i * i);
        }
    }
}