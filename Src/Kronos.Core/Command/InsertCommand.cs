using System.Net.Sockets;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;

namespace Kronos.Core.Command
{
    public class InsertCommand : BaseCommand
    {
        public RequestStatusCode Execute(IClientServerConnection service, InsertRequest request)
        {
            byte[] response = service.SendToServer(request);

            RequestStatusCode statusCode = SerializationUtils.Deserialize<RequestStatusCode>(response);

            return statusCode;
        }

        public override void ProcessRequest(Socket socket, byte[] requestBytes, IStorage storage)
        {
            InsertRequest insertRequest = SerializationUtils.Deserialize<InsertRequest>(requestBytes);
            storage.AddOrUpdate(insertRequest.Key, insertRequest.Object);

            SendToClient(socket, SerializationUtils.Serialize(RequestStatusCode.Ok));
        }
    }
}
