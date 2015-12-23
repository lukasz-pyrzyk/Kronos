using System.IO;
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
            byte[] package = ReadFully(request.Stream);
            Socket socket = null;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IPv4);
                socket.Connect(endPoint);

                socket.Send(package, 0, package.Length, SocketFlags.None);

                int receivedPackageLength = 0;
                while (receivedPackageLength == package.Length)
                {
                    receivedPackageLength = socket.Receive(package, sizeof (long), 0, SocketFlags.None);
                }
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

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
