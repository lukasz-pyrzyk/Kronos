using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class GetProcessor : CommandProcessor<GetRequest, byte[]>
    {
        public override RequestType Type { get; } = RequestType.Get;

        public override byte[] Process(ref GetRequest request, IStorage storage)
        {
            byte[] response;
            if (!storage.TryGet(request.Key, out response))
            {
                response = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            }

            return response;
        }
    }
}
