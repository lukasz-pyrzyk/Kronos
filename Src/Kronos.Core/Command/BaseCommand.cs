using Kronos.Core.Communication;
using Kronos.Core.Requests;

namespace Kronos.Core.Command
{
    public abstract class BaseCommand
    {
        private readonly IClientServerConnection _service;
        private readonly Request _request;

        protected BaseCommand(IClientServerConnection service, Request request)
        {
            _service = service;
            _request = request;
        }

        protected byte[] SendToServer()
        {
            return _service.SendToServer(_request);
        }
    }
}
