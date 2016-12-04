using System;
using System.Net.Sockets;
using System.Threading.Tasks;
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

        public async Task HandleAsync(RequestType requestType, byte[] request, int receivedBytes, Socket client)
        {
            switch (requestType)
            {
                case RequestType.Insert:
                    var insertRequest = SerializationUtils.Deserialize<InsertRequest>(request, receivedBytes);
                    await _insertProcessor.HandleAsync(insertRequest, _storage, client).ConfigureAwait(false);
                    break;
                case RequestType.Get:
                    var getRequest = SerializationUtils.Deserialize<GetRequest>(request, receivedBytes);
                    await _getProcessor.HandleAsync(getRequest, _storage, client).ConfigureAwait(false);
                    break;
                case RequestType.Delete:
                    var deleteRequest = SerializationUtils.Deserialize<DeleteRequest>(request, receivedBytes);
                    await _deleteProcessor.HandleAsync(deleteRequest, _storage, client).ConfigureAwait(false);
                    break;
                case RequestType.Count:
                    var countRequest = SerializationUtils.Deserialize<CountRequest>(request, receivedBytes);
                    await _countProcessor.HandleAsync(countRequest, _storage, client).ConfigureAwait(false);
                    break;
                case RequestType.Contains:
                    var containsRequest = SerializationUtils.Deserialize<ContainsRequest>(request, receivedBytes);
                    await _containsProcessor.HandleAsync(containsRequest, _storage, client).ConfigureAwait(false);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {requestType}");
            }
        }
    }
}
