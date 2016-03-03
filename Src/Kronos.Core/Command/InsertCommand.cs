using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;

namespace Kronos.Core.Command
{
    public class InsertCommand : BaseCommand
    {
        public InsertCommand(IClientServerConnection service, InsertRequest request) : base(service, request)
        {
        }

        public RequestStatusCode Execute()
        {
            byte[] response = SendToServer();

            RequestStatusCode statusCode = SerializationUtils.Deserialize<RequestStatusCode>(response);

            return statusCode;
        }
    }
}
