using System;
using System.IO;
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

        public void Handle(RequestType requestType, byte[] request, int receivedBytes, Stream responseStream)
        {
            switch (requestType)
            {
                case RequestType.Insert:
                    var insertRequest = SerializationUtils.Deserialize<InsertRequest>(request, receivedBytes);
                    bool insertResult = _insertProcessor.Process(ref insertRequest, _storage);
                    SerializationUtils.SerializeToStreamWithLength(responseStream, insertResult);
                    break;
                case RequestType.Get:
                    var getRequest = SerializationUtils.Deserialize<GetRequest>(request, receivedBytes);
                    byte[] getResult = _getProcessor.Process(ref getRequest, _storage);
                    SerializationUtils.SerializeToStreamWithLength(responseStream, getResult);
                    break;
                case RequestType.Delete:
                    var deleteRequest = SerializationUtils.Deserialize<DeleteRequest>(request, receivedBytes);
                    bool deleteResult = _deleteProcessor.Process(ref deleteRequest, _storage);
                    SerializationUtils.SerializeToStreamWithLength(responseStream, deleteResult);
                    break;
                case RequestType.Count:
                    var countRequest = SerializationUtils.Deserialize<CountRequest>(request, receivedBytes);
                    int countResult = _countProcessor.Process(ref countRequest, _storage);
                    SerializationUtils.SerializeToStreamWithLength(responseStream, countResult);
                    break;
                case RequestType.Contains:
                    var containsRequest = SerializationUtils.Deserialize<ContainsRequest>(request, receivedBytes);
                    bool containsResult = _containsProcessor.Process(ref containsRequest, _storage);
                    SerializationUtils.SerializeToStreamWithLength(responseStream, containsResult);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {requestType}");
            }
        }
    }
}
