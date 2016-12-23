using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ContainsProcessor : CommandProcessor<ContainsRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Contains;

        public override byte[] Process(ref ContainsRequest request, IStorage storage)
        {
            bool contains = storage.Contains(request.Key);

            return Reply(contains);
        }
    }
}
