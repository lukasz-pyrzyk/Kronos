using Google.Protobuf;
using Kronos.Core.Messages;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    public class GetProcessor : CommandProcessor<GetRequest, GetResponse>
    {
        public override GetResponse Reply(GetRequest request, IStorage storage)
        {
            if (storage.TryGet(request.Key, out ByteString package))
            {
                return new GetResponse { Exists = true, Data = package };
            }

            return new GetResponse { Exists = false, Data = ByteString.Empty };
        }
    }
}
