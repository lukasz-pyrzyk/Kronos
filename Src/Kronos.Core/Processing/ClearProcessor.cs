using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ClearProcessor : CommandProcessor<ClearRequest, bool>
    {
        public override byte[] Process(ref ClearRequest request, IStorage storage)
        {
            storage.Clear();

            return Reply(true);
        }
    }
}
