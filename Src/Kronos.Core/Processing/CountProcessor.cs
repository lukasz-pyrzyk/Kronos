using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class CountProcessor : CommandProcessor<CountRequest, int>
    {
        public override byte[] Process(ref CountRequest request, IStorage storage)
        {
            int count = storage.Count;

            return Reply(count);
        }
    }
}
