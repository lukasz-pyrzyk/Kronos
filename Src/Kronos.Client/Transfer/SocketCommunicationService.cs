using System;
using System.Net;
using System.Net.Sockets;
using Kronos.Shared.Requests;
using Kronos.Shared.Socket;
using Kronos.Shared.StatusCodes;
using NLog;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : ICommunicationService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public RequestStatusCode SendToNode(InsertRequest request, IPEndPoint endPoint)
        {
            Socket socket = null;
            byte[] packageToSend = SocketTransferUtil.GetTotalBytes(request.ObjectToCache);
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _logger.Debug("Connecting to the server socket");
                socket.Connect(endPoint);

                _logger.Debug($"Sending package of {packageToSend.Length} bytes");
                socket.Send(packageToSend, SocketFlags.None);

                int receivedValue = 0;
                while (receivedValue != packageToSend.Length)
                {
                    byte[] response = new byte[sizeof(int)];
                    socket.Receive(response, SocketFlags.None);
                    receivedValue = BitConverter.ToInt32(response, 0);
                }

                _logger.Debug($"Server has received {receivedValue} bytes");
            }
            catch (Exception ex)
            {
                _logger.Fatal($"During package transfer an error occurred {ex}");
                _logger.Debug("Returning information about exception");
                return RequestStatusCode.Failed;
            }
            finally
            {
                _logger.Debug("Disposing socket");
                socket?.Dispose();
            }

            return RequestStatusCode.Ok;
        }
    }
}
