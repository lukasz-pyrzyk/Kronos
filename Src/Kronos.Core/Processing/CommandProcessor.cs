using System.Threading.Tasks;
using Kronos.Core.Configuration;
using Kronos.Core.Networking;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public abstract class CommandProcessor<TRequest, TResponse> where TRequest : Google.Protobuf.IMessage
    {
        public abstract byte[] Process(TRequest request, IStorage storage);

        public async Task<TResponse> ExecuteAsync(TRequest request, IConnection service, ServerConfig server)
        {
            byte[] response = await service.SendAsync(request, server).ConfigureAwait(false);

            TResponse results = PrepareResponse<TResponse>(response);

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

        protected byte[] Reply(TResponse response)
        {
            return SerializationUtils.SerializeToStreamWithLength(response);
        }

        protected virtual T PrepareResponse<T>(byte[] responseBytes)
        {
            T results = SerializationUtils.Deserialize<T>(responseBytes);
            return results;
        }
    }
}
