using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Kronos.Core.Requests;
using Kronos.Core.StatusCodes;
using Kronos.Server.Storage;
using NLog;
using ProtoBuf;

namespace Kronos.Server.RequestProcessing
{
    internal class RequestProcessor : IRequestProcessor
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        public void ProcessRequest(Socket clientSocket, byte[] requestBytes)
        {
            _logger.Info($"Processing request of size {requestBytes.Length}");

            RequestType type;
            try
            {
                type = Deserialize<RequestType>(requestBytes.Take(sizeof(short)).ToArray());
            }
            catch (Exception ex)
            {
                _logger.Error($"Cannot find request type, exception: {ex}");
                throw new InvalidOperationException("Cannot find request type", ex);
            }

            switch (type)
            {
                case RequestType.InsertRequest:
                    InsertRequest insertRequest = Deserialize<InsertRequest>(requestBytes);
                    Process(insertRequest, clientSocket);
                    break;
                case RequestType.GetRequest:
                    GetRequest getRequest = Deserialize<GetRequest>(requestBytes);
                    Process(getRequest, clientSocket);
                    break;
            }
        }

        private void Process(InsertRequest request, Socket clientSocket)
        {
            InMemoryStorage.AddOrUpdate(request.ObjectToCache.Key, request.ObjectToCache.Object);
            RequestStatusCode code = RequestStatusCode.Ok;
            SendToSocket(clientSocket, BitConverter.GetBytes((short)code));
        }

        private void Process(GetRequest request, Socket clientSocket)
        {
            byte[] requestedObject = InMemoryStorage.TryGet(request.Key);
            SendToSocket(clientSocket, requestedObject);
        }

        private void SendToSocket(Socket clientSocket, byte[] buffer)
        {
            clientSocket.Send(buffer, buffer.Length, SocketFlags.None);
        }
        
        private T Deserialize<T>(byte[] request)
        {
            using (MemoryStream ms = new MemoryStream(request))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
