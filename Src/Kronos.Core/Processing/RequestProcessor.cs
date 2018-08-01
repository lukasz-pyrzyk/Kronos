using Kronos.Core.Messages;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly IStorage _storage;
        private readonly CommandProcessor<InsertRequest, InsertResponse> _insertProcessor;
        private readonly CommandProcessor<GetRequest, GetResponse> _getProcessor;
        private readonly CommandProcessor<DeleteRequest, DeleteResponse> _deleteProcessor;
        private readonly CommandProcessor<CountRequest, CountResponse> _countProcessor;
        private readonly CommandProcessor<ContainsRequest, ContainsResponse> _containsProcessor;
        private readonly CommandProcessor<ClearRequest, ClearResponse> _clearProcessor;
        private readonly CommandProcessor<StatsRequest, StatsResponse> _statsProcessor;

        public RequestProcessor(IStorage storage) : this(
            storage, new InsertProcessor(), new GetProcessor(),
            new DeleteProcessor(), new CountProcessor(), new ContainsProcessor(), new ClearProcessor(), new StatsProcessor()
            )
        { }

        internal RequestProcessor(
            IStorage storage,
            CommandProcessor<InsertRequest, InsertResponse> insertProcessor,
            CommandProcessor<GetRequest, GetResponse> getProcessor,
            CommandProcessor<DeleteRequest, DeleteResponse> deleteProcessor,
            CommandProcessor<CountRequest, CountResponse> countProcessor,
            CommandProcessor<ContainsRequest, ContainsResponse> containsProcessor,
            CommandProcessor<ClearRequest, ClearResponse> clearProcessor,
            CommandProcessor<StatsRequest, StatsResponse> statsProcessor
        )
        {
            _storage = storage;
            _insertProcessor = insertProcessor;
            _getProcessor = getProcessor;
            _deleteProcessor = deleteProcessor;
            _countProcessor = countProcessor;
            _containsProcessor = containsProcessor;
            _clearProcessor = clearProcessor;
            _statsProcessor = statsProcessor;
        }

        public Response Handle(Request request, Auth auth)
        {
            var response = new Response();

            bool authorized = auth.Authorize(request.Auth);
            if (!authorized)
            {
                response.Exception = $"User {request.Auth.Login} is not authorized";
                return response;
            }

            var internalResponse = request.InternalRequest;
            switch (internalResponse?.Type)
            {
                case RequestType.Insert:
                    response.InternalResponse = _insertProcessor.Reply((InsertRequest)internalResponse, _storage);
                    break;
                case RequestType.Get:
                    response.InternalResponse = _getProcessor.Reply((GetRequest)internalResponse, _storage);
                    break;
                case RequestType.Delete:
                    response.InternalResponse = _deleteProcessor.Reply((DeleteRequest)internalResponse, _storage);
                    break;
                case RequestType.Count:
                    response.InternalResponse = _countProcessor.Reply((CountRequest)internalResponse, _storage);
                    break;
                case RequestType.Contains:
                    response.InternalResponse = _containsProcessor.Reply((ContainsRequest)internalResponse, _storage);
                    break;
                case RequestType.Clear:
                    response.InternalResponse = _clearProcessor.Reply((ClearRequest)internalResponse, _storage);
                    break;
                case RequestType.Stats:
                    response.InternalResponse = _statsProcessor.Reply((StatsRequest)internalResponse, _storage);
                    break;
                default:
                    response.Exception = $"Request type {internalResponse?.Type} is not supported";
                    break;
            }

            return response;
        }
    }
}
