using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class GetProcessor : CommandProcessor<GetRequest, byte[]>
    {
        public override RequestType Type { get; } = RequestType.Get;

        public override async Task HandleAsync(GetRequest request, IStorage storage, Socket client)
        {
            byte[] response;
            if (!storage.TryGet(request.Key, out response))
            {
                response = SerializationUtils.Serialize(RequestStatusCode.NotFound);
            }

            await ReplyAsync(response, client);
        }
    }
}
