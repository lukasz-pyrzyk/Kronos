using Kronos.Core.Messages;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class GetProcessor : CommandProcessor<GetRequest, GetResponse>
    {
        public override GetResponse Reply(GetRequest request, IStorage storage)
        {
            if (storage.TryGet(request.Key, out var package))
            {
                return new GetResponse {  Data = package };
            }

            return new GetResponse();
        }
    }
}
