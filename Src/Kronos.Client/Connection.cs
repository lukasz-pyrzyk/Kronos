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
using Kronos.Core.Serialization;
using Polly;

namespace Kronos.Client
{
    public class Connection : IConnection
    {
        private const int RetryCount = 3;
        private static readonly Policy Policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(CreateExponentialBackoff(RetryCount));

        private readonly byte[] _sizeBytes = new byte[sizeof(int)];

        public async Task<Response> SendAsync(Request request, ServerConfig server)
        {
            Socket socket = null;
            Response response = null;
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
            }).ConfigureAwait(false);

            return response;
        }

        private async Task SendAsync(Request request, Socket server)
        {
            int size = request.CalculateSize();
            using (var stream = new SerializationStream(size))
            {
                request.Write(stream);
                stream.Flush();

                await SocketUtils.SendAllAsync(server, stream.MemoryWithLengthPrefix).ConfigureAwait(false);
            }
        }

        private async Task<Response> ReceiveAndDeserializeAsync(Socket socket)
        {
            await SocketUtils.ReceiveAllAsync(socket, _sizeBytes, _sizeBytes.Length).ConfigureAwait(false);
            int packageSize = BitConverter.ToInt32(_sizeBytes, 0);
            Array.Clear(_sizeBytes, 0, _sizeBytes.Length);

            byte[] requestBytes = ArrayPool<byte>.Shared.Rent(packageSize);
            try
            {
                await SocketUtils.ReceiveAllAsync(socket, requestBytes, packageSize).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(requestBytes);
            }

            Response response = new Response();
            response.Read(new DeserializationStream(requestBytes));

            return response;
        }

        private static IEnumerable<TimeSpan> CreateExponentialBackoff(int retryCount)
        {
            for (int i = 1; i <= retryCount; i++)
            {
                yield return TimeSpan.FromSeconds(i * i);
            }
        }
    }
}
