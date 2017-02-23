namespace Kronos.Core.Processing
{
    public interface IRequestProcessor
    {
        byte[] Handle(Request request);
    }
}
