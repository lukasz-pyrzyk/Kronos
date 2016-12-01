using System.Net.Sockets;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class CountProcessor : CommandProcessor<CountRequest, int>
    {
        public override RequestType Type { get; } = RequestType.Count;

        public override void Handle(ref CountRequest request, IStorage storage, Socket client)
        {
            int count = storage.Count;

            Reply(count, client);
        }
    }
}
