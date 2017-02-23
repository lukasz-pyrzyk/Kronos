using Google.Protobuf;
using Kronos.Core.Networking;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class GetProcessor : CommandProcessor<GetRequest, GetResponse>
    {
        public override GetResponse Reply(GetRequest request, IStorage storage)
        {
            byte[] package;
            if (storage.TryGet(request.Key, out package))
            {
                return new GetResponse { Exists = true, Data = ByteString.CopyFrom(package) };
            }

            return new GetResponse { Exists = false, Data = ByteString.Empty };
        }

        protected override GetResponse ParseResponse(Response response)
        {
            return response.GetRespone;
        }
    }
}
