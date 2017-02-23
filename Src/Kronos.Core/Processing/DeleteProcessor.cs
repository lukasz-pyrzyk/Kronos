using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class DeleteProcessor : CommandProcessor<DeleteRequest, bool>
    {
        public override byte[] Process(DeleteRequest request, IStorage storage)
        {
            bool deleted = storage.TryRemove(request.Key);

            return Reply(deleted);
        }
    }
}
