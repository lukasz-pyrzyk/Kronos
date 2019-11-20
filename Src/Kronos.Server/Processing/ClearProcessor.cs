using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    class ClearProcessor : CommandProcessor<ClearRequest, ClearResponse>
    {
        public override ClearResponse Reply(ClearRequest request, InMemoryStorage storage)
        {
            int deleted = storage.Clear();

            return new ClearResponse { Deleted = deleted };
        }
    }
}
