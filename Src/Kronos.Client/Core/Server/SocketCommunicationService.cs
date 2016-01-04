using System.Net;
using System.Net.Sockets;
using Kronos.Shared.Network.Codes;
using Kronos.Shared.Network.Requests;

namespace Kronos.Client.Core.Server
{
    public class SocketCommunicationService : ICommunicationService
    {
        public RequestStatusCode SendToNode(InsertRequest request, IPEndPoint endPoint)
        {
            Socket socket = null;
            byte[] packageToSend = request.ObjectToCache.SerializeNetworkPackage();
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IPv4);
                socket.Connect(endPoint);

                socket.Send(packageToSend, 0, packageToSend.Length, SocketFlags.None);

                int receivedPackageLength = 0;
                while (receivedPackageLength == packageToSend.Length)
                {
                    receivedPackageLength = socket.Receive(packageToSend, sizeof (long), 0, SocketFlags.None);
                }
            }
            catch (SocketException)
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
