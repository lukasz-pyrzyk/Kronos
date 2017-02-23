namespace Kronos.Core.Processing
{
    public interface IRequestProcessor
    {
        Response Handle(Request request);
    }
}
