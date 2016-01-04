using Kronos.Shared.Network.Model;

namespace Kronos.Shared.Network.Requests
{
    public class InsertRequest : Request
    {
        public CachedObject ObjectToCache { get; private set; }

        public InsertRequest(CachedObject objectToCache)
        {
            ObjectToCache = objectToCache;
        }
    }
}
