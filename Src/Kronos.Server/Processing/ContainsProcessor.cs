using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    class ContainsProcessor : CommandProcessor<ContainsRequest, ContainsResponse>
    {
        public override ContainsResponse Reply(ContainsRequest request, InMemoryStorage storage)
        {
            bool contains = storage.Contains(request.Key);

            return new ContainsResponse {Contains = contains};
        }
    }
}
