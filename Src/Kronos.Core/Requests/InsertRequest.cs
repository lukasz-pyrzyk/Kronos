using System;
using Kronos.Core.Model;

namespace Kronos.Core.Requests
{
    public class InsertRequest : Request
    {
        public CachedObject ObjectToCache { get; set; }

        // used by reflection
        public InsertRequest()
        {
        }

        public InsertRequest(CachedObject objectToCache)
        {
            ObjectToCache = objectToCache;
        }
    }
}
