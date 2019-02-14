using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public abstract class CommandProcessor<TRequest, TResponse>
        where TRequest : IMessage
        where TResponse : IMessage
    {
        public abstract TResponse Reply(TRequest request, IStorage storage);

        public async Task<TResponse> ExecuteAsync(Request request, IConnection service, ServerConfig server)
        {
            Response response = await service.SendAsync(request, server).ConfigureAwait(false);

            if (!response.Success)
            {
                throw new KronosException(response.Exception);
            }

            TResponse selectedResponse = SelectResponse(response);

            return selectedResponse;
        }

        protected abstract TResponse SelectResponse(Response response);
    }
}
