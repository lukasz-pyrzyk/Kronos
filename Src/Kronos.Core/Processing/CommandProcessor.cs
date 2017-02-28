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

            TResponse selectedResponse = ParseResponse(response);

            return selectedResponse;
        }

        public async Task<TResponse[]> ExecuteAsync(Request[] requests, IConnection[] services, ServerConfig[] servers)
        {
            int count = services.Length;
            Task<TResponse>[] responses = new Task<TResponse>[count];
            for (int i = 0; i < count; i++)
            {
                IConnection con = services[i];
                ServerConfig server = servers[i];
                Request request = requests[i];
                responses[i] = ExecuteAsync(request, con, server);
            }

            return await Task.WhenAll(responses);
        }

        protected abstract TResponse ParseResponse(Response response);
    }
}
