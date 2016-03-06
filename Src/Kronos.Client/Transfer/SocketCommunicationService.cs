using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Kronos.Core.Communication;
using Kronos.Core.Model.Exceptions;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : IClientServerConnection
    {
        private readonly IPEndPoint _host;

        private readonly Func<ISocket> _newSocketFunc;

        public SocketCommunicationService(IPEndPoint host) : this(host, () => new KronosSocket(AddressFamily.InterNetwork))
        {
        }

        internal SocketCommunicationService(IPEndPoint host, Func<ISocket> newSocketFunc)
        {
            _host = host;
            _newSocketFunc = newSocketFunc;
        }

        public byte[] SendToServer(Request request)
        {
            ISocket socket = _newSocketFunc();

            try
            {
                Trace.WriteLine("Connecting to the server socket");
                socket.Connect(_host);

                Trace.WriteLine("Sending request type");
                SentToClientAndWaitForConfirmation(socket, request.RequestType);

                Trace.WriteLine("Sending request");
                SentToClientAndWaitForConfirmation(socket, request);

                Trace.WriteLine("Waiting for response");
                byte[] requestBytes;
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
                            throw new KronosCommunicationException("Invalid tcp error. Socket has received more bytes than was specified.");
                    }

                    requestBytes = ms.ToArray();
                }
                return requestBytes;
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
        }

        private static void SentToClientAndWaitForConfirmation<T>(ISocket socket, T obj)
        {
            byte[] buffer = SerializationUtils.Serialize(obj);
            socket.Send(buffer);

            // wait for confirmation
            byte[] confirmationBuffer = new byte[SerializationUtils.Serialize(RequestStatusCode.Ok).Length];
            int count;
            while ((count = socket.Receive(confirmationBuffer)) == 0)
                Thread.Sleep(100);
        }
    }
}
