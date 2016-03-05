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
        private readonly IPEndPoint _nodeEndPoint;
        private const int bufferSize = 1024 * 1024 * 1024;

        public SocketCommunicationService(IPEndPoint host)
        {
            _nodeEndPoint = host;
        }

        public byte[] SendToServer(Request request)
        {
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Trace.WriteLine("Connecting to the server socket");
                socket.Connect(_nodeEndPoint);

                Trace.WriteLine("Sending request type");
                SentToClientAndWaitForConfirmation(socket, request.RequestType);

                Trace.WriteLine("Sending request");
                SentToClientAndWaitForConfirmation(socket, request);

                Trace.WriteLine("Waiting for response");
                byte[] requestBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[bufferSize];
                    using (NetworkStream stream = new NetworkStream(socket))
                    {
                        while (!stream.DataAvailable)
                        {
                            Thread.Sleep(300);
                        }

                        do
                        {
                            int received = stream.Read(buffer, 0, buffer.Length);
                            ms.Write(buffer, 0, received);
                        } while (stream.DataAvailable);
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
                    socket?.Dispose();
                }
                catch (SocketException)
                {
                }
            }
        }

        private static void SentToClientAndWaitForConfirmation<T>(Socket socket, T obj)
        {
            byte[] buffer = SerializationUtils.Serialize(obj);
            socket.Send(buffer, SocketFlags.None);

            // wait for confirmation
            byte[] confirmationBuffer = new byte[SerializationUtils.Serialize(RequestStatusCode.Ok).Length];
            int count;
            while ((count = socket.Receive(confirmationBuffer, SocketFlags.None)) == 0)
                Thread.Sleep(100);
        }
    }
}
