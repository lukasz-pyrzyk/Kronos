using Kronos.Core.Requests;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.Processors
{
    public class ContainsProcessor : CommandProcessor<ContainsRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Contains;

        public override void Handle(ref ContainsRequest request, IStorage storage, ISocket client)
        {
            bool contains = storage.Contains(request.Key);

            Reply(contains, client);
        }
    }
}
