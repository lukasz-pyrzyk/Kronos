using Kronos.Core.Requests;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.Processors
{
    public class InsertProcessor : CommandProcessor<InsertRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Insert;

        public override void Handle(ref InsertRequest request, IStorage storage, ISocket client)
        {
            storage.AddOrUpdate(request.Key, request.ExpiryDate, request.Object);

            Reply(true, client);
        }
    }
}
