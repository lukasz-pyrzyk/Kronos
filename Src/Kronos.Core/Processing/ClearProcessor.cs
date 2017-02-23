using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ClearProcessor : CommandProcessor<ClearRequest, ClearResponse>
    {
        public override ClearResponse Reply(ClearRequest request, IStorage storage)
        {
            storage.Clear();

            return new ClearResponse(); // todo use response...
        }

        protected override ClearResponse ParseResponse(Response response)
        {
            return response.ClearResponse;
        }
    }
}
