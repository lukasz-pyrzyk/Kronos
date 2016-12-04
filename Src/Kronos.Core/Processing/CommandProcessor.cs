using System.Net.Sockets;
using System.Threading.Tasks;
using Kronos.Core.Networking;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public abstract class CommandProcessor<TRequest, TResponse> where TRequest : IRequest
    {
        public abstract RequestType Type { get; }

        public abstract Task HandleAsync(TRequest request, IStorage storage, Socket client);

        public async Task<TResponse> ExecuteAsync(TRequest request, IConnection service)
        {
            byte[] response = await service.SendAsync(request);

            TResponse results = PrepareResponse<TResponse>(response);

            return results;
        }

        public async Task<TResponse[]> ExecuteAsync(TRequest request, IConnection[] services)
        {
            int count = services.Length;
            Task<TResponse>[] responses = new Task<TResponse>[count];
            for (int i = 0; i < count; i++)
            {
                IConnection connection = services[i];
                responses[i] = ExecuteAsync(request, connection);
            }

            return await Task.WhenAll(responses);
        }

        protected async Task ReplyAsync(TResponse response, Socket client)
        {
            byte[] data = SerializationUtils.SerializeToStreamWithLength(response);

            await SocketUtils.SendAllAsync(client, data);
        }

        protected virtual T PrepareResponse<T>(byte[] responseBytes)
        {
            T results = SerializationUtils.Deserialize<T>(responseBytes);
            return results;
        }
    }
}
