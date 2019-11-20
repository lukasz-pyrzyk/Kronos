using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, InsertResponse>
    {
        public override InsertResponse Reply(InsertRequest request, IStorage storage)
        {
            bool added = storage.Add(request.Key, request.Expiry?.ToDateTimeOffset(), request.Data);

            return new InsertResponse { Added = added };
        }
    }
}
