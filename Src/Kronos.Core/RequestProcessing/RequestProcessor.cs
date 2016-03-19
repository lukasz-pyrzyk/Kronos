using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.RequestProcessing
{
    public class RequestProcessor : IRequestProcessor
    {
        public void ProcessRequest(ISocket clientSocket, byte[] requestBytes, RequestType type, IStorage storage)
        {
            switch (type)
            {
                case RequestType.Insert:
                    InsertRequest insertRequest = SerializationUtils.Deserialize<InsertRequest>(requestBytes);
                    insertRequest.ProcessAndSendResponse(clientSocket, storage);
                    break;
                case RequestType.Get:
                    GetRequest getRequest = SerializationUtils.Deserialize<GetRequest>(requestBytes);
                    getRequest.ProcessAndSendResponse(clientSocket, storage);
                    break;
                case RequestType.Delete:
                    DeleteRequest deleteRequest = SerializationUtils.Deserialize<DeleteRequest>(requestBytes);
                    deleteRequest.ProcessAndSendResponse(clientSocket, storage);
                    break;
            }
        }
    }
}
