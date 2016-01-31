using System;
using System.Net;
using System.Net.Sockets;
using BinaryFormatter;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;
using NLog;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : ICommunicationService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly BinaryConverter _converter = new BinaryConverter();

        public RequestStatusCode SendToNode(InsertRequest request, IPEndPoint endPoint)
        {
            Socket socket = null;
            byte[] packageToSend = GeneratePackageWithTotalSize(request);
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

        private byte[] GeneratePackageWithTotalSize(InsertRequest request)
        {
            byte[] packageToSend = _converter.Serialize(request);
            byte[] packageSize = BitConverter.GetBytes(packageToSend.Length);

            byte[] packageToSendWithSize = new byte[sizeof(int) + packageSize.Length];
            Buffer.BlockCopy(packageSize, 0, packageToSendWithSize, 0, packageSize.Length);
            Buffer.BlockCopy(packageToSend, 0, packageToSendWithSize, packageSize.Length, packageToSend.Length);

            return packageToSendWithSize;
        }
    }
}
