using Kronos.Core.Messages;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, InsertResponse>
    {
        public override InsertResponse Reply(InsertRequest request, IStorage storage)
        {
            bool added = storage.Add(request.Key, request.Expiry, request.Data.ToArray()); // remove toarray

            return new InsertResponse { Added = added };
        }
    }
}
