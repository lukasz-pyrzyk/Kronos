using Kronos.Core.Requests;

namespace Kronos.Core.RequestProcessing
{
    public interface IRequestMapper
    {
        Request ProcessRequest(byte[] requestBytes, RequestType type);
    }
}
