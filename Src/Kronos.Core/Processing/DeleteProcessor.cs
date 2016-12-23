using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class DeleteProcessor : CommandProcessor<DeleteRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Delete;

        public override byte[] Process(ref DeleteRequest request, IStorage storage)
        {
            bool deleted = storage.TryRemove(request.Key);

            return Reply(deleted);
        }
    }
}
