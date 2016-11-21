using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Kronos.Core.Exceptions;
using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Polly;
using XGain.Sockets;

namespace Kronos.Client.Transfer
{
    public class Connection : IConnection
    {
        public int RetryCount => _timeSpans.Length;

        private readonly IPEndPoint _host;

        private readonly Func<ISocket> _newSocketFunc;

        private readonly TimeSpan[] _timeSpans;
        private readonly Policy _policy;

        public Connection(IPEndPoint host) : this(host, () => new XGainSocket(AddressFamily.InterNetwork))
        {
        }

        internal Connection(IPEndPoint host, Func<ISocket> newSocketFunc, int retryCount = 2)
        {
            _host = host;
            _newSocketFunc = newSocketFunc;
            var spans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                spans[i] = new TimeSpan(3 * retryCount);
            }
            _timeSpans = spans;
            _policy = Policy.Handle<Exception>().WaitAndRetry(_timeSpans);
        }

        public byte[] Send<TRequest>(TRequest request) where TRequest : IRequest
        {
            ISocket socket = _newSocketFunc();

            byte[] requestBytes = null;
            _policy.Execute(() =>
            {
                try
                {
                    Debug.WriteLine("Connecting to the server socket");
                    socket.Connect(_host);

                    Debug.WriteLine("Sending request");
                    SendToServer(request, socket);

                    Debug.WriteLine("Waiting for response");
                    requestBytes = ReceiveFromServer(socket);
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
            });

            return requestBytes;
        }

        private static void SendToServer(IRequest request, ISocket server)
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

            SocketUtils.SendAll(server, lengthBytes);
            SocketUtils.SendAll(server, data);
        }

        private static byte[] ReceiveFromServer(ISocket socket)
        {
            // todo array pool and stackalloc
            byte[] sizeBytes = new byte[sizeof(int)];
            SocketUtils.ReceiveAll(socket, sizeBytes, sizeBytes.Length);
            int size = BitConverter.ToInt32(sizeBytes, 0);
            Debug.Assert(size > 0);

            byte[] requestBytes = new byte[size];
            SocketUtils.ReceiveAll(socket, requestBytes, requestBytes.Length);

            return requestBytes;
        }
    }
}
