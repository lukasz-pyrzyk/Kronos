using Kronos.Core.Model;

namespace Kronos.Core.Requests
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
