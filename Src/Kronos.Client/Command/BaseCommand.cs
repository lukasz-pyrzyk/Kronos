using Kronos.Client.Transfer;
using Kronos.Core.Requests;

namespace Kronos.Client.Command
{
    public abstract class BaseCommand
    {
        private readonly ICommunicationService _service;
        private readonly Request _request;

        protected BaseCommand(ICommunicationService service, Request request)
        {
            _service = service;
            _request = request;
        }

        protected byte[] ExecuteCommand()
        {
            return _service.SendToNode(_request);
        }
    }
}
