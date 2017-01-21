using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Insert;

        public override bool Process(ref InsertRequest request, IStorage storage)
        {
            storage.AddOrUpdate(request.Key, request.ExpiryDate, request.Object);

            return true;
        }
    }
}
