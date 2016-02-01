using Kronos.Core.Model;
using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public class InsertRequest : Request
    {
        [ProtoMember(1)]
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
