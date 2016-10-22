using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Exceptions;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Polly;
using XGain.Sockets;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : IClientServerConnection
    {
        public static int RetryCount => _timeSpans.Length;

        private readonly IPEndPoint _host;

        private readonly Func<ISocket> _newSocketFunc;

        private static readonly TimeSpan[] _timeSpans = { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3) };
        private readonly Policy _policy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(_timeSpans);

        public SocketCommunicationService(IPEndPoint host) : this(host, () => new XGainSocket(AddressFamily.InterNetwork))
        {
        }

        internal SocketCommunicationService(IPEndPoint host, Func<ISocket> newSocketFunc)
        {
            _host = host;
            _newSocketFunc = newSocketFunc;
        }

        public async Task<byte[]> SendToServerAsync(Request request)
        {
            ISocket socket = _newSocketFunc();

            byte[] requestBytes = null;
            await _policy.ExecuteAsync(async () =>
            {
                try
                {
                    Trace.WriteLine("Connecting to the server socket");
                    socket.Connect(_host);

                    Trace.WriteLine("Sending request type");
                    await SentToClientAndWaitForConfirmation(socket, request.RequestType);

                    Trace.WriteLine("Sending request");
                    await SentToClientAndWaitForConfirmation(socket, request);

                    Trace.WriteLine("Waiting for response");
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

                            if (position > requestSize)
                                throw new KronosCommunicationException(
                                    "Invalid tcp error. Socket has received more bytes than was specified.");
                        }

                        requestBytes = ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"During package transfer an error occurred {ex}");
                    Trace.WriteLine("Returning information about exception");
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

        private static async Task SentToClientAndWaitForConfirmation<T>(ISocket socket, T obj)
        {
            byte[] buffer = SerializationUtils.Serialize(obj);
            socket.Send(buffer);

            // wait for confirmation
            byte[] confirmationBuffer = new byte[SerializationUtils.Serialize(RequestStatusCode.Ok).Length];
            int count;
            while ((count = socket.Receive(confirmationBuffer)) == 0)
                await Task.Delay(50);
        }
    }
}
