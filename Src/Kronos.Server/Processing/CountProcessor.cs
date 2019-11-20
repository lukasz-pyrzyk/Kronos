using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    public class CountProcessor : CommandProcessor<CountRequest, CountResponse>
    {
        public override CountResponse Reply(CountRequest request, IStorage storage)
        {
            int count = storage.Count;

            return new CountResponse { Count = count };
        }
    }
}
