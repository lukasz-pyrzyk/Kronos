using ProtoBuf;

namespace Kronos.Core.Requests
{
    [ProtoContract]
    public enum RequestType
    {
        [ProtoEnum]
        InsertRequest = 0,
        [ProtoEnum]
        GetRequest = 1,
    }
}
