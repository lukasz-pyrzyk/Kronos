using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;

namespace Kronos.Core.Command
{
    public class GetCommand : BaseCommand
    {
        public byte[] Execute(IClientServerConnection service, GetRequest request)
        {
            byte[] response = service.SendToServer(request);

            return response;
        }

        public override void ProcessRequest(Socket socket, byte[] requestBytes, IStorage storage)
        {
            GetRequest getRequest = SerializationUtils.Deserialize<GetRequest>(requestBytes);
            byte[] requestedObject = storage.TryGet(getRequest.Key);
            SendToClient(socket, requestedObject);
        }
    }
}
