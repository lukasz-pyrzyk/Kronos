using Kronos.Core.Communication;
using Kronos.Core.Serialization;
using Kronos.Core.StatusCodes;
using Kronos.Core.Storage;
using ProtoBuf;
using XGain.Sockets;

namespace Kronos.Core.Requests
{
    /// <summary>
    /// Base class for any request to the server
    /// </summary>
    [ProtoContract]
    [ProtoInclude(500, typeof(InsertRequest))]
    [ProtoInclude(1000, typeof(GetRequest))]
    public abstract class Request
    {
        public virtual RequestType RequestType { get; set; }

        public abstract void ProcessResponse(ISocket socket, IStorage storage);

        public T ProcessRequest<T>(IClientServerConnection service)
        {
            byte[] response = service.SendToServer(this);

            T results = ProcessFromClientCode<T>(response);

            return results;
        }

        protected virtual T ProcessFromClientCode<T>(byte[] responseBytes)
        {
            T results = SerializationUtils.Deserialize<T>(responseBytes);
            return results;
        }
    }
}
