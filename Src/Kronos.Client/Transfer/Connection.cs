using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Exceptions;
using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Polly;

namespace Kronos.Client.Transfer
{
    public class Connection : IConnection
    {
        private const int retryCount = 2;
        private static readonly Policy Policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(CreateExponentialBackoff(retryCount));

        private readonly IPEndPoint _host;
        
        public Connection(IPEndPoint host)
        {
            _host = host;
        }

        public async Task<byte[]> SendAsync<TRequest>(TRequest request)
            where TRequest : IRequest
        {
            Socket socket = null;
            byte[] response = null;
            await Policy.ExecuteAsync(async () =>
            {
                try
                {
                    Debug.WriteLine("Connecting to the server socket");
                    socket = new Socket(SocketType.Stream, ProtocolType.IP);
                    await socket.ConnectAsync(_host).ConfigureAwait(false);

                    Debug.WriteLine("Sending request");
                    await SendAsync(request, socket).ConfigureAwait(false);

                    Debug.WriteLine("Waiting for response");
                    response = await ReceiveAsync(socket).ConfigureAwait(false);

                    return response;
                }
                catch (Exception ex)
                {
                    throw new KronosCommunicationException($"Connection to the {_host} has been refused", ex);
                }
                finally
                {
                    socket?.Dispose();
                }
            }).ConfigureAwait(false);

            return response;
        }

        private static async Task SendAsync(IRequest request, Socket server)
        {
            // todo array pool and stackalloc
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationUtils.SerializeToStream(ms, request.Type);
                SerializationUtils.SerializeToStream(ms, request);
                data = ms.ToArray();
            }

            byte[] lengthBytes = new byte[sizeof(int)]; // stackalloc
            NoAllocBitConverter.GetBytes(data.Length, lengthBytes);

            await SocketUtils.SendAllAsync(server, lengthBytes).ConfigureAwait(false);
            await SocketUtils.SendAllAsync(server, data).ConfigureAwait(false);
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
