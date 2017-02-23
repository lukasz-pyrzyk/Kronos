using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, bool>
    {
        public override byte[] Process(InsertRequest request, IStorage storage)
        {
            // TODO remove ToDateTime
            // TODO remove ToByteArray
            // TODO rename properties...
            storage.AddOrUpdate(request.Key, request.Expiry.ToDateTime(), request.Data.ToByteArray());

            return Reply(true);
        }
    }
}
