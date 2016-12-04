using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class CountProcessor : CommandProcessor<CountRequest, int>
    {
        public override RequestType Type { get; } = RequestType.Count;

        public override async Task HandleAsync(CountRequest request, IStorage storage, Socket client)
        {
            int count = storage.Count;

            await ReplyAsync(count, client);
        }
    }
}
