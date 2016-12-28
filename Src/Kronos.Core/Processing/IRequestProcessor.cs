using System.IO;
using Kronos.Core.Requests;

namespace Kronos.Core.Processing
{
    public interface IRequestProcessor
    {
        void Handle(RequestType requestType, byte[] request, int received, Stream responseStream);
    }
}
