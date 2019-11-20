using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    class InsertProcessor : CommandProcessor<InsertRequest, InsertResponse>
    {
        public override InsertResponse Reply(InsertRequest request, InMemoryStorage storage)
        {
            bool added = storage.Add(request.Key, request.Expiry?.ToDateTimeOffset(), request.Data);

            return new InsertResponse { Added = added };
        }
    }
}
