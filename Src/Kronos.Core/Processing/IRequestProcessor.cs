namespace Kronos.Core.Processing
{
    public interface IRequestProcessor
    {
        byte[] Handle(RequestType requestType, byte[] request, int received);
    }
}
