using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ContainsProcessor : CommandProcessor<ContainsRequest, ContainsResponse>
    {
        public override ContainsResponse Reply(ContainsRequest request, IStorage storage)
        {
            bool contains = storage.Contains(request.Key);

            return new ContainsResponse(); // todo use real response
        }

        protected override ContainsResponse ParseResponse(Response response)
        {
            return response.ContainsResponse;
        }
    }
}
