using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    public class ContainsProcessor : CommandProcessor<ContainsRequest, ContainsResponse>
    {
        public override ContainsResponse Reply(ContainsRequest request, IStorage storage)
        {
            bool contains = storage.Contains(request.Key);

            return new ContainsResponse {Contains = contains};
        }
    }
}
