using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class DeleteProcessor : CommandProcessor<DeleteRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Delete;

        public override async Task HandleAsync(DeleteRequest request, IStorage storage, Socket client)
        {
            bool deleted = storage.TryRemove(request.Key);

            await ReplyAsync(deleted, client);
        }
    }
}
