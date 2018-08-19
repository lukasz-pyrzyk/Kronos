using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Exceptions;
using Kronos.Core.Messages;
using Kronos.Core.Networking;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public abstract class CommandProcessor<TRequest, TResponse>
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

        public async Task<TResponse[]> ExecuteAsync(Request[] requests, IConnection connection, ServerConfig[] servers)
        {
            Task<TResponse>[] responses = new Task<TResponse>[servers.Length];
            for (int i = 0; i < servers.Length; i++)
            {
                ServerConfig server = servers[i];
                Request request = requests[i];
                responses[i] = ExecuteAsync(request, connection, server);
            }

            return await Task.WhenAll(responses);
        }

        protected TResponse SelectResponse(Response response)
        {
            return (TResponse)response.InternalResponse;
        }
    }
}
