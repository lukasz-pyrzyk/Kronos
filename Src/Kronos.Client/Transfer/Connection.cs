using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
using Kronos.Core.Networking;
using Kronos.Core.Pooling;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Polly;

namespace Kronos.Client.Transfer
{
    public class Connection : IConnection
    {
        private const int RetryCount = 2;
        private static readonly Policy Policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(CreateExponentialBackoff(RetryCount));

        private readonly BufferedStream _stream = new BufferedStream();

        public async Task<byte[]> SendAsync<TRequest>(TRequest request, ServerConfig server)
            where TRequest : IRequest
        {
            Socket socket = null;
            byte[] response = null;
            await Policy.ExecuteAsync(async () =>
            {
                try
                {
                    Trace.WriteLine("Connecting to the server socket");
                    socket = new Socket(SocketType.Stream, ProtocolType.IP);
                    await socket.ConnectAsync(server.EndPoint).ConfigureAwait(false);

                    Trace.WriteLine("Sending request");
                    await SendAsync(request, socket).ConfigureAwait(false);

                    Trace.WriteLine("Waiting for response");
                    response = await ReceiveAsync(socket).ConfigureAwait(false);

                    return response;
                }
                catch (Exception ex)
                {
                    throw new KronosCommunicationException($"Connection to the {server.EndPoint} has been refused", ex);
                }
                finally
                {
                    if (!_stream.IsClean)
                    {
                        _stream.Clean();
                    }

                    socket?.Dispose();
                }
            }).ConfigureAwait(false);

            return response;
        }

        private async Task SendAsync(IRequest request, Socket server)
        {
            SerializationUtils.SerializeToStream(_stream, request.Type);
            SerializationUtils.SerializeToStream(_stream, request);

            await SocketUtils.SendAllAsync(server, _stream.RawBytes, (int)_stream.Length).ConfigureAwait(false);
        }

        private static async Task<byte[]> ReceiveAsync(Socket socket)
        {
            // todo array pool and stackalloc
            byte[] sizeBytes = new byte[sizeof(int)];
            await SocketUtils.ReceiveAllAsync(socket, sizeBytes, sizeBytes.Length).ConfigureAwait(false);
            int size = BitConverter.ToInt32(sizeBytes, 0);

            byte[] requestBytes = new byte[size];
            await SocketUtils.ReceiveAllAsync(socket, requestBytes, requestBytes.Length).ConfigureAwait(false);

            return requestBytes;
        }

        private static TimeSpan[] CreateExponentialBackoff(int retryCount)
        {
            var spans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                spans[i] = new TimeSpan(3 * retryCount);
            }

            return spans;
        }
    }
}
