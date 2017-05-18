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

        public RequestProcessor(IStorage storage) : this(
            storage, new InsertProcessor(), new GetProcessor(),
            new DeleteProcessor(), new CountProcessor(), new ContainsProcessor(), new ClearProcessor()
            )
        { }

        internal RequestProcessor(
            IStorage storage,
            CommandProcessor<InsertRequest, InsertResponse> insertProcessor,
            CommandProcessor<GetRequest, GetResponse> getProcessor,
            CommandProcessor<DeleteRequest, DeleteResponse> deleteProcessor,
            CommandProcessor<CountRequest, CountResponse> countProcessor,
            CommandProcessor<ContainsRequest, ContainsResponse> containsProcessor,
            CommandProcessor<ClearRequest, ClearResponse> clearProcessor
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

        public Response Handle(Request request, Auth auth)
        {
            var response = new Response();

            bool authorized = auth.Authorize(request.Auth);
            if (!authorized)
            {
                response.Exception = $"User {request.Auth.Login} is not authorized";
                return response;
            }

            switch (request.Type)
            {
                case RequestType.Insert:
                    response.InsertResponse = _insertProcessor.Reply(request.InsertRequest, _storage);
                    break;
                case RequestType.Get:
                    response.GetRespone = _getProcessor.Reply(request.GetRequest, _storage);
                    break;
                case RequestType.Delete:
                    response.DeleteResponse = _deleteProcessor.Reply(request.DeleteRequest, _storage);
                    break;
                case RequestType.Count:
                    response.CountResponse = _countProcessor.Reply(request.CountRequest, _storage);
                    break;
                case RequestType.Contains:
                    response.ContainsResponse = _containsProcessor.Reply(request.ContainsRequest, _storage);
                    break;
                case RequestType.Clear:
                    response.ClearResponse = _clearProcessor.Reply(request.ClearRequest, _storage);
                    break;
                default:
                    response.Exception = $"Request type {request.Type} is not supported";
                    break;
            }

            return response;
        }
    }
}
