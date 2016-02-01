using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;
using NLog;
using ProtoBuf;

namespace Kronos.Client.Transfer
{
    public class SocketCommunicationService : ICommunicationService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public RequestStatusCode SendToNode(InsertRequest request, IPEndPoint endPoint)
        {
            byte[] packageToSend = GeneratePackageWithTotalSize(request);
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _logger.Debug("Connecting to the server socket");
                socket.Connect(endPoint);

                _logger.Debug($"Sending package of {packageToSend.Length} bytes");
                socket.Send(packageToSend, SocketFlags.None);

                RequestStatusCode requestStatus = RequestStatusCode.Processing;
                while (requestStatus == RequestStatusCode.Processing)
                {
                    byte[] response = new byte[sizeof(short)];
                    socket.Receive(response, SocketFlags.None);
                    requestStatus = (RequestStatusCode)BitConverter.ToInt16(response, 0);
                }

                _logger.Debug($"Server has received {requestStatus} status");
            }
            catch (Exception ex)
            {
                _logger.Fatal($"During package transfer an error occurred {ex}");
                _logger.Debug("Returning information about exception");
                return RequestStatusCode.Failed;
            }

            return RequestStatusCode.Ok;
        }

        private byte[] GeneratePackageWithTotalSize(InsertRequest request)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(ms, request, PrefixStyle.Fixed32);
                return ms.ToArray();
            }
        }
    }
}
