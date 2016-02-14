using Kronos.Client.Transfer;
using Kronos.Core.Requests;

namespace Kronos.Client.Command
{
    public class GetCommand : BaseCommand
    {
        public GetCommand(ICommunicationService service, GetRequest request) : base(service, request)
        {
        }

        public byte[] Execute()
        {
            byte[] response = ExecuteCommand();

            return response;
        }
    }
}
