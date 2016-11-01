using Kronos.Core.Requests;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.Processors
{
    public class DeleteProcessor : CommandProcessor<DeleteRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Delete;

        public override void Handle(ref DeleteRequest request, IStorage storage, ISocket client)
        {
            bool deleted = storage.TryRemove(request.Key);

            Reply(deleted, client);
        }
    }
}
