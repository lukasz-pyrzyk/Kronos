using System.IO;
using Kronos.Core.Messages;

namespace Kronos.Server
{
    public interface ISocketProcessor
    {
        Request ReceiveRequest(Stream client);

        void SendResponse(Stream client, Response response);
    }
}
