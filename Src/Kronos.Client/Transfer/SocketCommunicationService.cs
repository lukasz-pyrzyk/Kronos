using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Core.Exceptions;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Polly;
using XGain.Sockets;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : IClientServerConnection
    {
        public int RetryCount => _timeSpans.Length;

        private readonly IPEndPoint _host;

        private readonly Func<ISocket> _newSocketFunc;

        private readonly TimeSpan[] _timeSpans;
        private readonly Policy _policy;

        public SocketCommunicationService(IPEndPoint host) : this(host, () => new XGainSocket(AddressFamily.InterNetwork))
        {
        }

        internal SocketCommunicationService(IPEndPoint host, Func<ISocket> newSocketFunc, int retryCount = 2)
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
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] requestSizeBytes = new byte[sizeof(int)];
                        int position = socket.Receive(requestSizeBytes);
                        int requestSize = SerializationUtils.GetLengthOfPackage(requestSizeBytes);

                        ms.Write(requestSizeBytes, 0, position);
                        position = 0;
                        while (position != requestSize)
                        {
                            byte[] package = new byte[socket.BufferSize];
                            int received = socket.Receive(package);
                            position += received;

                            ms.Write(package, 0, received);
                        }

                        requestBytes = ms.ToArray();
                    }
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
    }
}
