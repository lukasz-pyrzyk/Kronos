using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    public class ClearProcessor : CommandProcessor<ClearRequest, ClearResponse>
    {
        public override ClearResponse Reply(ClearRequest request, IStorage storage)
        {
            int deleted = storage.Clear();

            return new ClearResponse { Deleted = deleted };
        }
    }
}
