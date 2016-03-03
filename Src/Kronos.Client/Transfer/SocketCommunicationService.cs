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

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : IClientServerConnection
    {
        private readonly IPEndPoint _nodeEndPoint;
        private const int bufferSize = 1024 * 8;

        public SocketCommunicationService(IPEndPoint host)
        {
            _nodeEndPoint = host;
        }

        public byte[] SendToServer(Request request)
        {
            byte[] packageToSend = SerializationUtils.Serialize(request);
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Trace.WriteLine("Connecting to the server socket");
                socket.Connect(_nodeEndPoint);

                Trace.WriteLine($"Sending package of {packageToSend.Length} bytes");
                socket.Send(packageToSend, SocketFlags.None);

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
    }
}
