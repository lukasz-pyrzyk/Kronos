using System;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.Processors
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly IStorage _storage;
        private readonly CommandProcessor<InsertRequest, bool> _insertProcessor;
        private readonly CommandProcessor<GetRequest, byte[]> _getProcessor;
        private readonly CommandProcessor<DeleteRequest, bool> _deleteProcessor;
        private readonly CommandProcessor<CountRequest, int> _countProcessor;
        private readonly CommandProcessor<ContainsRequest, bool> _containsProcessor;

        public RequestProcessor(IStorage storage) : this(
            storage, new InsertProcessor(), new GetProcessor(),
            new DeleteProcessor(), new CountProcessor(), new ContainsProcessor()
            )
        { }

        internal RequestProcessor(
            IStorage storage,
            CommandProcessor<InsertRequest, bool> insertProcessor,
            CommandProcessor<GetRequest, byte[]> getProcessor,
            CommandProcessor<DeleteRequest, bool> deleteProcessor,
            CommandProcessor<CountRequest, int> countProcessor,
            CommandProcessor<ContainsRequest, bool> containsProcessor
        )
        {
            _storage = storage;
            _insertProcessor = insertProcessor;
            _getProcessor = getProcessor;
            _deleteProcessor = deleteProcessor;
            _countProcessor = countProcessor;
            _containsProcessor = containsProcessor;
        }

        public void HandleIncomingRequest(RequestType requestType, byte[] request, int receivedBytes, ISocket client)
        {
            switch (requestType)
            {
                case RequestType.Insert:
                    var insertRequest = SerializationUtils.Deserialize<InsertRequest>(request, receivedBytes);
                    _insertProcessor.Handle(ref insertRequest, _storage, client);
                    break;
                case RequestType.Get:
                    var getRequest = SerializationUtils.Deserialize<GetRequest>(request, receivedBytes);
                    _getProcessor.Handle(ref getRequest, _storage, client);
                    break;
                case RequestType.Delete:
                    var deleteRequest = SerializationUtils.Deserialize<DeleteRequest>(request, receivedBytes);
                    _deleteProcessor.Handle(ref deleteRequest, _storage, client);
                    break;
                case RequestType.Count:
                    var countRequest = SerializationUtils.Deserialize<CountRequest>(request, receivedBytes);
                    _countProcessor.Handle(ref countRequest, _storage, client);
                    break;
                case RequestType.Contains:
                    var containsRequest = SerializationUtils.Deserialize<ContainsRequest>(request, receivedBytes);
                    _containsProcessor.Handle(ref containsRequest, _storage, client);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {requestType}");
            }
        }
    }
}
