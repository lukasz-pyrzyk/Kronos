using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;
using NLog;
using ProtoBuf;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : ICommunicationService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public RequestStatusCode SendToNode(Request request, IPEndPoint endPoint)
        {
            byte[] packageToSend = GeneratePackageWithTotalSize(request);
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _logger.Debug("Connecting to the server socket");
                socket.Connect(endPoint);

                _logger.Debug($"Sending package of {packageToSend.Length} bytes");
                socket.Send(packageToSend, SocketFlags.None);
            }
            catch (Exception ex)
            {
                _logger.Fatal($"During package transfer an error occurred {ex}");
                _logger.Debug("Returning information about exception");
                return RequestStatusCode.Failed;
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

            return RequestStatusCode.Ok;
        }

        private byte[] GeneratePackageWithTotalSize(Request request)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, request);
                return ms.ToArray();
            }
        }
    }
}
