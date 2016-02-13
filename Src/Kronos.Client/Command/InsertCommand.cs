using Kronos.Client.Transfer;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;

namespace Kronos.Client.Command
{
    public class InsertCommand : BaseCommand
    {
        public InsertCommand(ICommunicationService service, InsertRequest request) : base(service, request)
        {
        }

        public RequestStatusCode Execute()
        {
            byte[] response = ExecuteCommand();

            RequestStatusCode statusCode = SerializationUtils.Deserialize<RequestStatusCode>(response);

            return statusCode;
        }
    }
}
