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
                case RequestType.InsertRequest:
                    InsertRequest insertRequest = SerializationUtils.Deserialize<InsertRequest>(requestBytes);
                    insertRequest.ProcessResponse(clientSocket, storage);
                    break;
                case RequestType.GetRequest:
                    GetRequest getRequest = SerializationUtils.Deserialize<GetRequest>(requestBytes);
                    getRequest.ProcessResponse(clientSocket, storage);
                    break;
            }
        }
    }
}
