using System.Threading.Tasks;
using Kronos.Core.Communication;
using Kronos.Core.Requests;
using Kronos.Core.Serialization;
using Kronos.Core.Storage;
using XGain.Sockets;

namespace Kronos.Core.Processors
{
    public abstract class CommandProcessor<TRequest, TResponse> where TRequest : IRequest
    {
        public abstract RequestType Type { get; }

        public abstract void Handle(ref TRequest request, IStorage storage, ISocket client);

        public async Task<TResponse> ExecuteAsync(TRequest request, IClientServerConnection service)
        {
            byte[] response = await Task.Run(() => service.Send(request)).ConfigureAwait(false);

            TResponse results = PrepareResponse<TResponse>(response);

            return results;
        }

        protected void Reply(TResponse response, ISocket client)
        {
            byte[] data = SerializationUtils.SerializeToStreamWithLength(response);

            SocketUtils.SendAll(client, data);
        }

        protected virtual T PrepareResponse<T>(byte[] responseBytes)
        {
            T results = SerializationUtils.DeserializeWithLength<T>(responseBytes);
            return results;
        }
    }
}
