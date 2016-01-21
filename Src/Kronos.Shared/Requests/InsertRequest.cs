using Kronos.Shared.Model;

namespace Kronos.Shared.Requests
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
