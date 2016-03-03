using System;
using System.Linq;
using System.Net.Sockets;
using Kronos.Core.Command;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
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
                    InsertCommand insertCommand = new InsertCommand();
                    insertCommand.ProcessRequest(clientSocket, requestBytes, storage);
                    break;
                case RequestType.GetRequest:
                    GetCommand getCommand = new GetCommand();
                    getCommand.ProcessRequest(clientSocket, requestBytes, storage);
                    break;
            }
        }
    }
}
