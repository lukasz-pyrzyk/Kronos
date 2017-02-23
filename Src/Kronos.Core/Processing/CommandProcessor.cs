using System.Threading.Tasks;
using Google.Protobuf;
using Kronos.Core.Configuration;
using Kronos.Core.Networking;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public abstract class CommandProcessor<TRequest, TResponse>
        where TRequest : IMessage
        where TResponse : IMessage
    {
        public abstract TResponse Reply(TRequest request, IStorage storage);

        public async Task<TResponse> ExecuteAsync(TRequest request, IConnection service, ServerConfig server)
        {
            byte[] package = await service.SendAsync(request, server).ConfigureAwait(false);
            Response response = Response.Parser.ParseFrom(package);

            TResponse results = ParseResponse(response);

            return results;
        }

        public async Task<TResponse[]> ExecuteAsync(TRequest request, IConnection[] services, ServerConfig[] servers)
        {
            int count = services.Length;
            Task<TResponse>[] responses = new Task<TResponse>[count];
            for (int i = 0; i < count; i++)
            {
                IConnection con = services[i];
                ServerConfig server = servers[i];
                responses[i] = ExecuteAsync(request, con, server);
            }

            return await Task.WhenAll(responses);
        }

        protected abstract TResponse ParseResponse(Response response);
    }
}
