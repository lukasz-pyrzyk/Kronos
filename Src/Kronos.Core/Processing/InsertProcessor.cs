using System.Net.Sockets;
using Kronos.Core.Requests;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class InsertProcessor : CommandProcessor<InsertRequest, bool>
    {
        public override RequestType Type { get; } = RequestType.Insert;

        public override void Handle(ref InsertRequest request, IStorage storage, Socket client)
        {
            storage.AddOrUpdate(request.Key, request.ExpiryDate, request.Object);

            Reply(true, client);
        }
    }
}
