using System;
using Google.Protobuf;
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

        public byte[] Handle(Request request)
        {
            switch (request.Type)
            {
                case RequestType.Insert:
                    return _insertProcessor.Process(request.InsertRequest, _storage);
                case RequestType.Get:
                    return _getProcessor.Process(request.GetRequest, _storage);
                case RequestType.Delete:
                    return _deleteProcessor.Process(request.DeleteRequest, _storage);
                case RequestType.Count:
                    return _countProcessor.Process(request.CountRequest, _storage);
                case RequestType.Contains:
                    return _containsProcessor.Process(request.ContainsRequest, _storage);
                case RequestType.Clear:
                    return _clearProcessor.Process(request.ClearRequest, _storage);
                default:
                    throw new InvalidOperationException($"Cannot find processor for type {request.Type}");
            }
        }
    }
}
