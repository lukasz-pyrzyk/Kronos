using System;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly IStorage _storage;
        private readonly CommandProcessor<InsertRequest, bool> _insertProcessor;
        private readonly CommandProcessor<GetRequest, byte[]> _getProcessor;
        private readonly CommandProcessor<DeleteRequest, bool> _deleteProcessor;
        private readonly CommandProcessor<CountRequest, int> _countProcessor;
        private readonly CommandProcessor<ContainsRequest, bool> _containsProcessor;
        private readonly CommandProcessor<ClearRequest, bool> _clearProcessor;

        public RequestProcessor(IStorage storage) : this(
            storage, new InsertProcessor(), new GetProcessor(),
            new DeleteProcessor(), new CountProcessor(), new ContainsProcessor(), new ClearProcessor()
            )
        { }

        internal RequestProcessor(
            IStorage storage,
            CommandProcessor<InsertRequest, bool> insertProcessor,
            CommandProcessor<GetRequest, byte[]> getProcessor,
            CommandProcessor<DeleteRequest, bool> deleteProcessor,
            CommandProcessor<CountRequest, int> countProcessor,
            CommandProcessor<ContainsRequest, bool> containsProcessor,
            CommandProcessor<ClearRequest, bool> clearProcessor
        )
        {
            _storage = storage;
            _insertProcessor = insertProcessor;
            _getProcessor = getProcessor;
            _deleteProcessor = deleteProcessor;
            _countProcessor = countProcessor;
            _containsProcessor = containsProcessor;
            _clearProcessor = clearProcessor;
        }

        public byte[] Handle(RequestType requestType, byte[] request, int receivedBytes)
        {
            switch (requestType)
            {
                case RequestType.Insert:
                    var insertRequest = SerializationUtils.Deserialize<InsertRequest>(request, receivedBytes);
                    return _insertProcessor.Process(ref insertRequest, _storage);
                case RequestType.Get:
                    var getRequest = SerializationUtils.Deserialize<GetRequest>(request, receivedBytes);
                    return _getProcessor.Process(ref getRequest, _storage);
                case RequestType.Delete:
                    var deleteRequest = SerializationUtils.Deserialize<DeleteRequest>(request, receivedBytes);
                    return _deleteProcessor.Process(ref deleteRequest, _storage);
                case RequestType.Count:
                    var countRequest = SerializationUtils.Deserialize<CountRequest>(request, receivedBytes);
                    return _countProcessor.Process(ref countRequest, _storage);
                case RequestType.Contains:
                    var containsRequest = SerializationUtils.Deserialize<ContainsRequest>(request, receivedBytes);
                    return _containsProcessor.Process(ref containsRequest, _storage);
                case RequestType.Clear:
                    var clearRequest = SerializationUtils.Deserialize<ClearRequest>(request, receivedBytes);
                    return _clearProcessor.Process(ref clearRequest, _storage);
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {requestType}");
            }
        }
    }
}
