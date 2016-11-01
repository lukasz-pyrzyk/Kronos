using Kronos.Core.Requests;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.Processors
{
    public class CountProcessor : CommandProcessor<CountRequest, int>
    {
        public override RequestType Type { get; } = RequestType.Count;

        public override void Handle(ref CountRequest request, IStorage storage, ISocket client)
        {
            int count = storage.Count;

            Reply(count, client);
        }
    }
}
