using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public enum RequestType : short
    {
        [ProtoEnum]
        InsertRequest = 1,
        [ProtoEnum]
        GetRequest = 2,
    }
}
