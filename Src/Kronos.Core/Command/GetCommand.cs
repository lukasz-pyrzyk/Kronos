using System;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;

namespace Kronos.Core.Command
{
    public class GetCommand : BaseCommand
    {
        public byte[] Execute(IClientServerConnection service, GetRequest request)
        {
            byte[] response = service.SendToServer(request);

            try
            {
                // if server returned NotFound status code, return null
                RequestStatusCode notFound = SerializationUtils.Deserialize<RequestStatusCode>(response);
                if (notFound == RequestStatusCode.NotFound)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
            }

            return response;
        }

        public override void ProcessRequest(ISocket socket, byte[] requestBytes, IStorage storage)
        {
            GetRequest getRequest = SerializationUtils.Deserialize<GetRequest>(requestBytes);
            byte[] requestedObject = storage.TryGet(getRequest.Key) ?? SerializationUtils.Serialize(RequestStatusCode.NotFound);
            SendToClient(socket, requestedObject);
        }
    }
}
