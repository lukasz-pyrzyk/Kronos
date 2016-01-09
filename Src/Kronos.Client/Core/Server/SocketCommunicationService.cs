using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Requests;
using Kronos.Shared.Socket;

namespace Kronos.Client.Core.Server
{
    public class SocketCommunicationService : ICommunicationService
    {
        public RequestStatusCode SendToNode(InsertRequest request, IPEndPoint endPoint)
        {
            Socket socket = null;
            byte[] packageToSend = SocketTransferUtil.GetTotalBytes(request.ObjectToCache);
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                socket.Send(packageToSend, SocketFlags.None);

                int receivedValue = 0;
                while (receivedValue != packageToSend.Length)
                {
                    byte[] response = new byte[sizeof(int)];
                    socket.Receive(response, SocketFlags.None);
                    receivedValue = BitConverter.ToInt32(response, 0);
                }
                socket.Dispose();
            }
            catch (SocketException ex)
            {
                return RequestStatusCode.Failed;
            }
            finally
            {
                socket?.Dispose();
            }

            return RequestStatusCode.Ok;
        }
    }
}
