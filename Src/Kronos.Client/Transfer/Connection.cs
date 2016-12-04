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
        public int RetryCount => _timeSpans.Length;

        private readonly IPEndPoint _host;

        private readonly Func<Socket> _newSocketFunc;

        private readonly TimeSpan[] _timeSpans;
        private readonly Policy _policy;

        public Connection(IPEndPoint host) : this(host, () => new Socket(SocketType.Stream, ProtocolType.IP))
        {
        }

        internal Connection(IPEndPoint host, Func<Socket> newSocketFunc, int retryCount = 2)
        {
            _host = host;
            _newSocketFunc = newSocketFunc;
            var spans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                spans[i] = new TimeSpan(3 * retryCount);
            }
            _timeSpans = spans;
            _policy = Policy.Handle<Exception>().WaitAndRetryAsync(_timeSpans);
        }

        public async Task<byte[]> SendAsync<TRequest>(TRequest request) where TRequest : IRequest
        {
            Socket socket = _newSocketFunc();

            byte[] requestBytes = null;
            await _policy.ExecuteAsync(async () =>
            {
                try
                {
                    Debug.WriteLine("Connecting to the server socket");
                    await socket.ConnectAsync(_host).ConfigureAwait(false);

                    Debug.WriteLine("Sending request");
                    await SendToServerAsync(request, socket).ConfigureAwait(false);

                    Debug.WriteLine("Waiting for response");
                    requestBytes = await ReceiveFromServerAsync(socket).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"During package transfer an error occurred {ex}");
                    Debug.WriteLine("Returning information about exception");
                    throw new KronosCommunicationException(ex.Message, ex);
                }
                finally
                {
                    try
                    {
                        socket.Dispose();
                    }
                    catch (SocketException)
                    {
                    }
                }
            }).ConfigureAwait(false);

            return requestBytes;
        }

        private static async Task SendToServerAsync(IRequest request, Socket server)
        {
            // todo array pool and stackalloc
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationUtils.SerializeToStream(ms, request.Type);
                SerializationUtils.SerializeToStream(ms, request);
                data = ms.ToArray();
            }

            byte[] lengthBytes = new byte[4]; // stackalloc
            NoAllocBitConverter.GetBytes(data.Length, lengthBytes);

            await SocketUtils.SendAllAsync(server, lengthBytes).ConfigureAwait(false);
            await SocketUtils.SendAllAsync(server, data).ConfigureAwait(false);
        }

        private static async Task<byte[]> ReceiveFromServerAsync(Socket socket)
        {
            // todo array pool and stackalloc
            byte[] sizeBytes = new byte[sizeof(int)];
            await SocketUtils.ReceiveAllAsync(socket, sizeBytes, sizeBytes.Length).ConfigureAwait(false);
            int size = BitConverter.ToInt32(sizeBytes, 0);

            byte[] requestBytes = new byte[size];
            await SocketUtils.ReceiveAllAsync(socket, requestBytes, requestBytes.Length).ConfigureAwait(false);

            return requestBytes;
        }
    }
}
