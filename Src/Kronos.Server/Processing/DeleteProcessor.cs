using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    class DeleteProcessor : CommandProcessor<DeleteRequest, DeleteResponse>
    {
        public override DeleteResponse Reply(DeleteRequest request, InMemoryStorage storage)
        {
            bool deleted = storage.TryRemove(request.Key);

            return new DeleteResponse { Deleted = deleted };
        }
    }
}
