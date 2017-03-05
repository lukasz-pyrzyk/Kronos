using Kronos.Core.Messages;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ContainsProcessor : CommandProcessor<ContainsRequest, ContainsResponse>
    {
        public override ContainsResponse Reply(ContainsRequest request, IStorage storage)
        {
            bool contains = storage.Contains(request.Key);

            return new ContainsResponse {Contains = contains};
        }

        protected override ContainsResponse SelectResponse(Response response)
        {
            return response.ContainsResponse;
        }
    }
}
