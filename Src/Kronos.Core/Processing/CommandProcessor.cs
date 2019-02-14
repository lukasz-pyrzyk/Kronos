using Google.Protobuf;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public abstract class CommandProcessor<TRequest, TResponse>
        where TRequest : IMessage
        where TResponse : IMessage
    {
        public abstract TResponse Reply(TRequest request, IStorage storage);
    }
}
