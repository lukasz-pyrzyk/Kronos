using System.Net.Sockets;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class DeleteProcessor : CommandProcessor<DeleteRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Delete;

        public override void Handle(ref DeleteRequest request, IStorage storage, Socket client)
        {
            bool deleted = storage.TryRemove(request.Key);

            Reply(deleted, client);
        }
    }
}
