using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    public class DeleteProcessor : CommandProcessor<DeleteRequest, DeleteResponse>
    {
        public override DeleteResponse Reply(DeleteRequest request, IStorage storage)
        {
            bool deleted = storage.TryRemove(request.Key);

            return new DeleteResponse { Deleted = deleted };
        }
    }
}
