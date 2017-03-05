using Kronos.Core.Messages;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, InsertResponse>
    {
        public override InsertResponse Reply(InsertRequest request, IStorage storage)
        {
            // TODO remove ToDateTime
            // TODO rename properties...
            bool added = storage.Add(request.Key, request.Expiry?.ToDateTime(), request.Data);

            return new InsertResponse { Added = added };
        }

        protected override InsertResponse SelectResponse(Response response)
        {
            return response.InsertResponse;
        }
    }
}
