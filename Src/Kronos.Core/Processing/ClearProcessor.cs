using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ClearProcessor : CommandProcessor<ClearRequest, ClearResponse>
    {
        public override ClearResponse Reply(ClearRequest request, IStorage storage)
        {
            int deleted = storage.Clear();

            return new ClearResponse { Deleted = deleted };
        }

        protected override ClearResponse ParseResponse(Response response)
        {
            return response.ClearResponse;
        }
    }
}
