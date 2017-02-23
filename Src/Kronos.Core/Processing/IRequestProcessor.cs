using Kronos.Core.Messages;

namespace Kronos.Core.Processing
{
    public interface IRequestProcessor
    {
        Response Handle(Request request);
    }
}
