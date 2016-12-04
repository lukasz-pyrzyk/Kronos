using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Insert;

        public override async Task HandleAsync(InsertRequest request, IStorage storage, Socket client)
        {
            storage.AddOrUpdate(request.Key, request.ExpiryDate, request.Object);

            await ReplyAsync(true, client).ConfigureAwait(false);
        }
    }
}
