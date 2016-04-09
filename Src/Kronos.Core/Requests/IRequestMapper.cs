namespace Kronos.Core.Requests
{
    public interface IRequestMapper
    {
        Request ProcessRequest(byte[] requestBytes, RequestType type);
    }
}
