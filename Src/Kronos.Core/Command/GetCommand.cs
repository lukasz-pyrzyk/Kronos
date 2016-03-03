using Kronos.Core.Communication;
using Kronos.Core.Requests;

namespace Kronos.Core.Command
{
    public class GetCommand : BaseCommand
    {
        public GetCommand(IClientServerConnection service, GetRequest request) : base(service, request)
        {
        }

        public byte[] Execute()
        {
            byte[] response = SendToServer();

            return response;
        }
    }
}
