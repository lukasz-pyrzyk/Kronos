using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ContainsProcessor : CommandProcessor<ContainsRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Contains;

        public override async Task HandleAsync(ContainsRequest request, IStorage storage, Socket client)
        {
            bool contains = storage.Contains(request.Key);

            await ReplyAsync(contains, client);
        }
    }
}
