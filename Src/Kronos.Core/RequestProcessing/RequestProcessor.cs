using System;
using System.Linq;
using System.Net.Sockets;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using NLog;

namespace Kronos.Server.RequestProcessing
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        public void ProcessRequest(Socket clientSocket, byte[] requestBytes, IStorage storage)
        {
            _logger.Info($"Processing request of size {requestBytes.Length}");

            RequestType type;
            try
            {
                type = SerializationUtils.Deserialize<RequestType>(requestBytes.Take(sizeof(short)).ToArray());
            }
            catch (Exception ex)
            {
                _logger.Error($"Cannot find request type, exception: {ex}");
                throw new InvalidOperationException("Cannot find request type", ex);
            }

            switch (type)
            {
                case RequestType.InsertRequest:
                    InsertRequest insertRequest = SerializationUtils.Deserialize<InsertRequest>(requestBytes);
                    Process(insertRequest, clientSocket, storage);
                    break;
                case RequestType.GetRequest:
                    GetRequest getRequest = SerializationUtils.Deserialize<GetRequest>(requestBytes);
                    Process(getRequest, clientSocket, storage);
                    break;
            }
        }

        private void Process(InsertRequest request, Socket clientSocket, IStorage storage)
        {
            storage.AddOrUpdate(request.ObjectToCache.Key, request.ObjectToCache.Object);
            SendToSocket(clientSocket, SerializationUtils.Serialize(RequestStatusCode.Ok));
        }

        private void Process(GetRequest request, Socket clientSocket, IStorage storage)
        {
            byte[] requestedObject = storage.TryGet(request.Key);
            SendToSocket(clientSocket, requestedObject);
        }

        private void SendToSocket(Socket clientSocket, byte[] buffer)
        {
            clientSocket.Send(buffer, buffer.Length, SocketFlags.None);
        }
    }
}
