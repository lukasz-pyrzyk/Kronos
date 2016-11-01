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

        public void HandleIncomingRequest(RequestType requestType, byte[] request, ISocket client)
        {
            switch (requestType)
            {
                case RequestType.Insert:
                    var insertRequest = Deserialize<InsertRequest>(request);
                    _insertProcessor.Handle(ref insertRequest, _storage, client);
                    break;
                case RequestType.Get:
                    var getRequest = Deserialize<GetRequest>(request);
                    _getProcessor.Handle(ref getRequest, _storage, client);
                    break;
                case RequestType.Delete:
                    var deleteRequest = Deserialize<DeleteRequest>(request);
                    _deleteProcessor.Handle(ref deleteRequest, _storage, client);
                    break;
                case RequestType.Count:
                    var countRequest = Deserialize<CountRequest>(request);
                    _countProcessor.Handle(ref countRequest, _storage, client);
                    break;
                case RequestType.Contains:
                    var containsRequest = Deserialize<ContainsRequest>(request);
                    _containsProcessor.Handle(ref containsRequest, _storage, client);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {requestType}");
            }
        }

        private TRequest Deserialize<TRequest>(byte[] request)
        {
            return SerializationUtils.Deserialize<TRequest>(request);
        }
    }
}
