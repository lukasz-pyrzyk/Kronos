using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Command
{
    public abstract class BaseCommand
    {
        private readonly IClientServerConnection _service;
        private readonly Request _request;

        public abstract void ProcessRequest(Socket socket, byte[] requestBytes, IStorage storage);

        protected BaseCommand()
        {
        }

        protected BaseCommand(IClientServerConnection service, Request request)
        {
            _service = service;
            _request = request;
        }

        protected byte[] SendToServer()
        {
            return _service.SendToServer(_request);
        }

        protected static void SendToClient(Socket clientSocket, byte[] buffer)
        {
            clientSocket.Send(buffer, buffer.Length, SocketFlags.None);
        }
    }
}
