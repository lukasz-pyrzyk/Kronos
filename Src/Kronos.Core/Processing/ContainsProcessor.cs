using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ContainsProcessor : CommandProcessor<ContainsRequest, bool>
    {
        public override byte[] Process(ContainsRequest request, IStorage storage)
        {
            bool contains = storage.Contains(request.Key);

            return Reply(contains);
        }
    }
}
