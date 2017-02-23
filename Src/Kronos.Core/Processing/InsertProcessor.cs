using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, InsertResponse>
    {
        public override InsertResponse Reply(InsertRequest request, IStorage storage)
        {
            // TODO remove ToDateTime
            // TODO remove ToByteArray
            // TODO rename properties...
            storage.AddOrUpdate(request.Key, request.Expiry.ToDateTime(), request.Data.ToByteArray());

            return new InsertResponse(); // todo new response...
        }

        protected override InsertResponse ParseResponse(Response response)
        {
            return response.InsertResponse;
        }
    }
}
