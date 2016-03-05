using Kronos.Core.Command;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Storage;
using NLog;

namespace Kronos.Core.RequestProcessing
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        public void ProcessRequest(ISocket clientSocket, byte[] requestBytes, RequestType type, IStorage storage)
        {
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
