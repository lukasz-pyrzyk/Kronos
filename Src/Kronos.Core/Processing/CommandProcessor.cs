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

        public abstract TResponse Process(ref TRequest request, IStorage storage);

        public async Task<TResponse> ExecuteAsync(TRequest request, IConnection service)
        {
            byte[] response = await service.SendAsync(request).ConfigureAwait(false);

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

        protected virtual T PrepareResponse<T>(byte[] responseBytes)
        {
            T results = SerializationUtils.Deserialize<T>(responseBytes);
            return results;
        }
    }
}
