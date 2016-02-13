using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using NLog;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : ICommunicationService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private const int bufferSize = 1024 * 8;

        public byte[] SendToNode(Request request, IPEndPoint endPoint)
        {
            byte[] packageToSend = SerializationUtils.Serialize(request);
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _logger.Debug("Connecting to the server socket");
                socket.Connect(endPoint);

                _logger.Debug($"Sending package of {packageToSend.Length} bytes");
                socket.Send(packageToSend, SocketFlags.None);

                _logger.Debug("Waiting for response");
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
                _logger.Fatal($"During package transfer an error occurred {ex}");
                _logger.Debug("Returning information about exception");
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

            return null;
        }
    }
}
